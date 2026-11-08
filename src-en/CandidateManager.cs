using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace UniPinyin
{
    public class CandidateManager
    {
        private string _buffer = "";
        private List<string> _allCandidates = new List<string>();
        private int _pageIndex = 0;
        private CancellationTokenSource _cts = null;

        public event Action<string, List<string>, int, int> OnCandidatesUpdated;
        public event Action OnBufferCleared;

        public string CurrentBuffer
        {
            get { return _buffer; }
        }

        public bool HasBuffer
        {
            get { return !string.IsNullOrEmpty(_buffer); }
        }

        public void AppendChar(char c, Settings settings)
        {
            if (_buffer.Length >= 100) return; // Support long sentences (up to 100 chars)
            bool handled;
            _buffer = PinyinToneHelper.ProcessKey(_buffer, c, out handled);
            _pageIndex = 0;
            RefreshCandidates(settings);
        }

        public void RemoveLastChar(Settings settings)
        {
            if (_buffer.Length > 0)
            {
                _buffer = _buffer.Substring(0, _buffer.Length - 1);
                _pageIndex = 0;

                if (_buffer.Length == 0)
                {
                    Clear();
                }
                else
                {
                    RefreshCandidates(settings);
                }
            }
        }

        public void Clear()
        {
            _buffer = "";
            _allCandidates.Clear();
            _pageIndex = 0;
            CancelPendingGoogleQuery();

            if (OnBufferCleared != null)
            {
                OnBufferCleared();
            }
        }

        public void NextPage(int pageSize)
        {
            int totalPages = Math.Max(1, (int)Math.Ceiling((double)_allCandidates.Count / pageSize));
            if (_pageIndex + 1 < totalPages)
            {
                _pageIndex++;
                NotifyUpdate(pageSize);
            }
        }

        public void PrevPage(int pageSize)
        {
            if (_pageIndex > 0)
            {
                _pageIndex--;
                NotifyUpdate(pageSize);
            }
        }

        public bool CommitCandidateByIndex(int relativeIndex, int pageSize)
        {
            int actualIndex = (_pageIndex * pageSize) + relativeIndex;
            if (actualIndex >= 0 && actualIndex < _allCandidates.Count)
            {
                string chosen = _allCandidates[actualIndex];
                NativeMethods.SendUnicodeString(chosen);
                Clear();
                return true;
            }
            return false;
        }

        public void CommitRaw()
        {
            if (!string.IsNullOrEmpty(_buffer))
            {
                NativeMethods.SendUnicodeString(_buffer);
            }
            Clear();
        }

        private void RefreshCandidates(Settings settings)
        {
            CancelPendingGoogleQuery();

            string currentRaw = _buffer;
            string currentQuery = PinyinToneHelper.StripTones(_buffer);
            int reqCount = Math.Max(10, settings.CandidateCount * 2);

            // 1. Instant local dictionary lookup
            List<string> localList = LocalDictionary.Lookup(currentQuery, settings.IsSimplified, reqCount);
            _allCandidates = new List<string>(localList);

            NotifyUpdate(settings.CandidateCount);

            // 2. Asynchronous Google Input Tools suggestion
            if (settings.UseGoogleOnline)
            {
                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Thread.Sleep(50);
                        if (token.IsCancellationRequested) return;

                        List<string> googleList = GooglePinyinService.Query(currentQuery, settings.IsSimplified, reqCount);

                        if (token.IsCancellationRequested) return;

                        if (googleList != null && googleList.Count > 0)
                        {
                            List<string> merged = new List<string>(googleList);
                            foreach (var item in localList)
                            {
                                if (!merged.Contains(item)) merged.Add(item);
                            }

                            if (_buffer == currentRaw)
                            {
                                _allCandidates = merged;
                                NotifyUpdate(settings.CandidateCount);
                            }
                        }
                    }
                    catch { }
                }, token);
            }
        }

        private void CancelPendingGoogleQuery()
        {
            if (_cts != null)
            {
                try
                {
                    _cts.Cancel();
                    _cts.Dispose();
                }
                catch { }
                _cts = null;
            }
        }

        private void NotifyUpdate(int pageSize)
        {
            if (OnCandidatesUpdated != null)
            {
                OnCandidatesUpdated(_buffer, _allCandidates, _pageIndex, pageSize);
            }
        }
    }
}
