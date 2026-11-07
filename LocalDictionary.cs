using System;
using System.Collections.Generic;
using System.Text;

namespace UniPinyin
{
    public static class LocalDictionary
    {
        private static readonly Dictionary<string, List<string>> Dict = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<char, char> SimpToTrad = new Dictionary<char, char>();

        static LocalDictionary()
        {
            InitDictionary();
            InitSimpToTrad();
        }

        public static List<string> Lookup(string pinyin, bool isSimplified, int maxCount)
        {
            if (string.IsNullOrEmpty(pinyin)) return new List<string>();

            List<string> results = new List<string>();

            // 1. Direct match
            if (Dict.ContainsKey(pinyin))
            {
                foreach (var w in Dict[pinyin])
                {
                    if (results.Count >= maxCount) break;
                    string word = isSimplified ? w : ConvertToTraditional(w);
                    if (!results.Contains(word)) results.Add(word);
                }
            }

            // 2. Prefix match for multi-character words
            if (results.Count < maxCount)
            {
                foreach (var kvp in Dict)
                {
                    if (results.Count >= maxCount) break;
                    if (kvp.Key.StartsWith(pinyin, StringComparison.OrdinalIgnoreCase) && !kvp.Key.Equals(pinyin, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var w in kvp.Value)
                        {
                            if (results.Count >= maxCount) break;
                            string word = isSimplified ? w : ConvertToTraditional(w);
                            if (!results.Contains(word)) results.Add(word);
                        }
                    }
                }
            }

            return results;
        }

        public static string ConvertToTraditional(string simplified)
        {
            if (string.IsNullOrEmpty(simplified)) return simplified;
            StringBuilder sb = new StringBuilder(simplified.Length);
            foreach (char c in simplified)
            {
                if (SimpToTrad.ContainsKey(c)) sb.Append(SimpToTrad[c]);
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private static void Add(string pinyin, params string[] words)
        {
            if (!Dict.ContainsKey(pinyin))
            {
                Dict[pinyin] = new List<string>();
            }
            foreach (var w in words)
            {
                if (!Dict[pinyin].Contains(w)) Dict[pinyin].Add(w);
            }
        }

        private static void InitDictionary()
        {
            // Common words & greetings
            Add("nihao", "你好", "拟好", "您好", "妮好");
            Add("ninhao", "您好");
            Add("xiexie", "谢谢", "写写", "歇歇");
            Add("zaijian", "再见", "在建");
            Add("zaoan", "早安");
            Add("wanan", "晚安");
            Add("duibuqi", "对不起");
            Add("meiguanxi", "没关系");
            Add("meishi", "没事", "没事儿");
            Add("bukechi", "不客气", "不用谢");
            Add("buyongxie", "不用谢");
            Add("buke", "不可", "不客气");
            Add("weishenme", "为什么", "为甚么");
            Add("zenmeyang", "怎么样", "怎样");
            Add("zenme", "怎么", "怎样");
            Add("zenyang", "怎样", "怎样做");
            Add("shenme", "什么", "甚么");
            Add("shei", "谁");
            Add("shui", "水", "睡", "谁", "税", "说");
            Add("nali", "哪里", "那里");
            Add("nazi", "哪儿", "那儿");
            Add("zheli", "这里", "者里");
            Add("zhe", "这", "着", "者");
            Add("na", "那", "哪", "拿", "纳", "呐");
            Add("women", "我们", "倭门");
            Add("nimen", "你们");
            Add("tamen", "他们", "她们", "它们");
            Add("dajia", "大家", "大伙", "大甲");
            Add("zhongwen", "中文", "中温");
            Add("hanyu", "汉语", "韩语", "含蓄");
            Add("zhongguo", "中国", "重过");
            Add("meiguo", "美国");
            Add("riben", "日本");
            Add("hanguo", "韩国");
            Add("yingguo", "英国");
            Add("faguo", "法国");
            Add("deguo", "德国");
            Add("beijing", "北京", "背景");
            Add("shanghai", "上海");
            Add("guangzhou", "广州");
            Add("shenzhen", "深圳");
            Add("taibei", "台北");
            Add("xianggang", "香港");

            Add("pengyou", "朋友", "澎友");
            Add("laoshi", "老师", "老实", "老市");
            Add("xuesheng", "学生");
            Add("tongxue", "同学");
            Add("tongshi", "同事", "同时");
            Add("gongzuo", "工作", "公作");
            Add("xuexi", "学习", "雪西");
            Add("xuexiao", "学校");
            Add("daxue", "大学");
            Add("zhongxue", "中学");
            Add("xiaoxue", "小学");
            Add("gongsi", "公司");
            Add("shouji", "手机", "手记");
            Add("diannao", "电脑", "典闹");
            Add("wangluo", "网络", "网路");
            Add("ruanjian", "软件");
            Add("yingjian", "硬件");
            Add("chengxu", "程序", "秩序");
            Add("shurufa", "输入法");
            Add("pinyin", "拼音", "聘引");
            Add("hanzi", "汉字", "汉子");

            Add("jintian", "今天", "紧填");
            Add("mingtian", "明天");
            Add("zuotian", "昨天");
            Add("houtian", "后天");
            Add("qiantian", "前天");
            Add("xianzai", "现在", "限在");
            Add("shijian", "时间", "事件", "石坚");
            Add("nian", "年", "念", "粘");
            Add("yue", "月", "约", "阅", "越", "悦");
            Add("ri", "日", "入");
            Add("tian", "天", "田", "填", "添");
            Add("dian", "点", "电", "店", "典", "垫");
            Add("fen", "分", "份", "纷", "粉", "奋");
            Add("miao", "秒", "苗", "妙", "庙");
            Add("xingqi", "星期");
            Add("zhoumo", "周末");
            Add("shangban", "上班");
            Add("xiaban", "下班");
            Add("fangjia", "放假", "房价");

            Add("chifan", "吃饭");
            Add("hecha", "喝茶");
            Add("hekafei", "喝咖啡");
            Add("shuijiao", "睡觉");
            Add("qichuang", "起床");
            Add("huijia", "回家");
            Add("chumen", "出门");
            Add("gouwu", "购物");
            Add("maicai", "买菜");
            Add("duoshaoqian", "多少钱");
            Add("duoshao", "多少");
            Add("tai", "太", "态", "泰", "台");
            Add("taiguile", "太贵了");
            Add("pianyi", "便宜");
            Add("yidianr", "一点儿", "一点");
            Add("yidian", "一点", "译电");

            Add("xihuan", "喜欢", "西环");
            Add("ai", "爱", "哎", "矮", "挨");
            Add("xiang", "想", "像", "向", "相", "响", "香");
            Add("zhidao", "知道", "指导");
            Add("renshi", "认识");
            Add("mingbai", "明白");
            Add("dong", "懂", "动", "冬", "东", "洞");
            Add("liaojie", "了解");
            Add("gaoxing", "高兴");
            Add("kuaile", "快乐");
            Add("xingfu", "幸福");
            Add("jiankang", "健康");
            Add("pingan", "平安");
            Add("shengri", "生日");
            Add("shengrikuaile", "生日快乐");
            Add("xinnian", "新年");
            Add("xinniankuaile", "新年快乐");
            Add("gongxifacai", "恭喜发财");

            Add("wo", "我", "窝", "握", "卧", "喔");
            Add("ni", "你", "拟", "泥", "逆", "倪");
            Add("ta", "他", "她", "它", "踏", "塔");
            Add("men", "们", "门", "闷", "门儿");
            Add("de", "的", "地", "得", "德");
            Add("di", "地", "第", "低", "底", "帝");
            Add("le", "了", "乐", "勒");
            Add("ma", "吗", "妈", "马", "骂", "麻");
            Add("ba", "吧", "八", "把", "爸", "拔");
            Add("ne", "呢", "讷");
            Add("a", "啊", "阿", "呵");
            Add("ou", "偶", "欧", "呕", "鸥");
            Add("ya", "呀", "鸭", "牙", "压", "雅");

            Add("shi", "是", "时", "事", "十", "实", "使", "世", "市", "识", "师", "示", "式", "视");
            Add("bu", "不", "步", "部", "布", "补", "捕");
            Add("bushi", "不是");
            Add("you", "有", "又", "由", "右", "油", "友", "游", "优");
            Add("meiyou", "没有");
            Add("mei", "没", "美", "每", "妹", "煤");
            Add("zai", "在", "再", "载", "灾");
            Add("zhengzai", "正在");
            Add("qu", "去", "取", "区", "曲", "趣");
            Add("lai", "来", "赖", "睐");
            Add("dao", "到", "道", "导", "倒", "岛");
            Add("shuo", "说", "硕", "朔");
            Add("shuohua", "说话");
            Add("kan", "看", "堪", "砍");
            Add("kandao", "看到", "看见");
            Add("kanjian", "看见");
            Add("ting", "听", "停", "厅", "挺");
            Add("tingdao", "听到", "听见");
            Add("tingjian", "听见");
            Add("xie", "写", "些", "谢", "斜", "鞋");
            Add("xiezhi", "写字");
            Add("du", "读", "度", "独", "都", "堵");
            Add("dushu", "读书");
            Add("chi", "吃", "持", "尺", "齿", "赤");
            Add("he", "喝", "和", "合", "何", "盒");
            Add("mai", "买", "卖", "迈", "埋");
            Add("yong", "用", "永", "勇", "拥");
            Add("gei", "给");
            Add("wen", "问", "文", "闻", "温", "稳");
            Add("da", "大", "打", "达", "答");
            Add("xiao", "小", "笑", "消", "效", "校");
            Add("duo", "多", "朵", "夺", "躲");
            Add("shao", "少", "烧", "梢", "哨");
            Add("hao", "好", "号", "毫", "豪", "耗");
            Add("huai", "坏", "怀", "淮");
            Add("leng", "冷", "棱");
            Add("re", "热", "惹");
            Add("kuai", "快", "块", "筷", "脍");
            Add("man", "慢", "满", "漫", "蛮");
            Add("yuan", "远", "元", "原", "圆", "愿", "院");
            Add("jin", "进", "近", "金", "今", "斤", "尽");

            // Numbers
            Add("yi", "一", "已", "以", "意", "义", "易", "医", "依", "仪");
            Add("er", "二", "儿", "耳", "而", "尔");
            Add("san", "三", "散", "伞");
            Add("si", "四", "死", "思", "司", "斯", "丝");
            Add("wu", "五", "无", "物", "武", "舞", "屋", "误");
            Add("liu", "六", "流", "留", "刘", "柳");
            Add("qi", "七", "起", "期", "其", "气", "奇", "齐", "妻");
            Add("ba", "八", "吧", "把", "爸", "拔");
            Add("jiu", "九", "就", "酒", "旧", "救", "久");
            Add("shi", "十", "是", "事", "时", "实");
            Add("ling", "零", "领", "令", "灵", "另");
            Add("bai", "百", "白", "摆", "拜");
            Add("qian", "千", "前", "钱", "浅", "签");
            Add("wan", "万", "玩", "完", "晚", "碗");
            Add("yiwan", "一万");
            Add("yibai", "一百");
            Add("yiqian", "一千");

            // Standard Pinyin Syllables
            Add("ai", "爱", "矮", "挨", "哎", "癌");
            Add("an", "安", "按", "暗", "岸", "案");
            Add("ang", "昂");
            Add("ao", "奥", "傲", "熬", "凹");
            Add("bai", "百", "白", "拜", "摆", "败");
            Add("ban", "半", "办", "班", "般", "板", "搬");
            Add("bang", "帮", "棒", "榜", "旁", "绑");
            Add("bao", "包", "抱", "报", "保", "宝", "饱", "暴");
            Add("bei", "北", "被", "杯", "倍", "备", "背");
            Add("ben", "本", "笨", "奔");
            Add("beng", "蹦", "崩", "泵");
            Add("bi", "比", "笔", "必", "闭", "避", "币", "彼");
            Add("bian", "边", "变", "便", "编", "扁");
            Add("biao", "表", "标", "飙", "标");
            Add("bie", "别", "憋", "瘪");
            Add("bin", "宾", "滨", "彬");
            Add("bing", "冰", "并", "病", "兵", "饼");
            Add("bo", "波", "播", "伯", "博", "拨", "脖");
            Add("bu", "不", "部", "步", "布", "补", "捕");
            Add("ca", "擦");
            Add("cai", "才", "采", "彩", "菜", "财", "猜");
            Add("can", "参", "餐", "残", "惨");
            Add("cang", "藏", "仓", "苍");
            Add("cao", "草", "操", "曹", "槽");
            Add("ce", "策", "侧", "测", "册");
            Add("cen", "参");
            Add("ceng", "曾", "层", "蹭");
            Add("cha", "茶", "查", "差", "插", "叉");
            Add("chai", "柴", "拆");
            Add("chan", "产", "阐", "颤", "单");
            Add("chang", "长", "场", "常", "唱", "畅", "尝");
            Add("chao", "超", "朝", "吵", "潮", "炒");
            Add("che", "车", "彻", "扯", "撤");
            Add("chen", "晨", "沉", "陈", "衬");
            Add("cheng", "成", "程", "城", "承", "称", "诚", "呈");
            Add("chi", "吃", "持", "迟", "尺", "赤", "池");
            Add("chong", "充", "重", "虫", "冲", "崇");
            Add("chou", "抽", "仇", "愁", "丑", "筹");
            Add("chu", "出", "初", "除", "处", "楚", "储");
            Add("chuai", "揣");
            Add("chuan", "穿", "传", "船", "川");
            Add("chuang", "窗", "床", "创", "闯");
            Add("chui", "吹", "垂", "锤");
            Add("chun", "春", "纯", "唇", "蠢");
            Add("chuo", "戳");
            Add("ci", "此", "次", "词", "辞", "刺", "磁");
            Add("cong", "从", "聪", "葱", "匆");
            Add("cou", "凑");
            Add("cu", "粗", "促", "醋", "簇");
            Add("cuan", "窜", "篡");
            Add("cui", "催", "翠", "脆", "崔");
            Add("cun", "村", "存", "寸");
            Add("cuo", "错", "措", "挫");
            Add("da", "大", "打", "达", "答", "搭");
            Add("dai", "带", "代", "待", "袋", "戴", "贷");
            Add("dan", "但", "单", "蛋", "担", "淡", "弹");
            Add("dang", "当", "党", "挡", "荡");
            Add("dao", "到", "道", "倒", "岛", "导", "刀");
            Add("de", "的", "得", "地", "德");
            Add("deng", "等", "灯", "登", "邓");
            Add("di", "地", "第", "低", "底", "帝", "弟");
            Add("dian", "点", "电", "店", "典", "垫", "殿");
            Add("diao", "调", "掉", "吊", "钓");
            Add("die", "爹", "跌", "叠", "碟");
            Add("ding", "定", "订", "顶", "丁", "钉");
            Add("diu", "丢");
            Add("dong", "东", "动", "冬", "懂", "洞", "冻");
            Add("dou", "都", "斗", "豆", "抖", "逗");
            Add("du", "度", "读", "独", "都", "毒", "堵");
            Add("duan", "段", "短", "断", "端");
            Add("dui", "对", "队", "堆", "兑");
            Add("dun", "顿", "吨", "蹲", "盾");
            Add("duo", "多", "朵", "夺", "躲", "堕");
            Add("e", "俄", "恶", "额", "鹅", "饿");
            Add("en", "恩", "嗯");
            Add("er", "二", "儿", "耳", "而", "尔");
            Add("fa", "发", "法", "罚", "乏");
            Add("fan", "反", "饭", "犯", "范", "烦", "繁", "翻");
            Add("fang", "方", "房", "放", "防", "访", "仿");
            Add("fei", "非", "飞", "费", "废", "肥");
            Add("fen", "分", "份", "纷", "粉", "奋", "愤");
            Add("feng", "风", "封", "丰", "奉", "峰", "疯");
            Add("fo", "佛");
            Add("fou", "否");
            Add("fu", "服", "复", "父", "负", "富", "副", "付", "府");
            Add("ga", "咖", "嘎");
            Add("gai", "该", "改", "盖", "概");
            Add("gan", "感", "干", "赶", "敢", "甘");
            Add("gang", "刚", "港", "钢", "纲");
            Add("gao", "高", "告", "搞", "稿", "糕");
            Add("ge", "个", "各", "歌", "哥", "格", "割");
            Add("gei", "给");
            Add("gen", "跟", "根");
            Add("geng", "更", "耕", "耿");
            Add("gong", "公", "工", "共", "功", "供", "攻");
            Add("gou", "够", "购", "狗", "构", "勾");
            Add("gu", "古", "骨", "故", "顾", "股", "孤");
            Add("gua", "瓜", "挂", "刮");
            Add("guai", "怪", "拐", "乖");
            Add("guan", "关", "管", "官", "馆", "观", "惯");
            Add("guang", "光", "广", "逛");
            Add("gui", "贵", "规", "鬼", "归", "轨");
            Add("gun", "滚", "棍");
            Add("guo", "过", "国", "果", "裹", "郭");
            Add("ha", "哈", "蛤");
            Add("hai", "海", "还", "孩", "害", "咳");
            Add("han", "汉", "含", "喊", "寒", "汗");
            Add("hang", "行", "航", "杭");
            Add("hao", "好", "号", "毫", "豪", "耗");
            Add("he", "和", "喝", "合", "何", "盒", "河", "贺");
            Add("hei", "黑", "嘿");
            Add("hen", "很", "恨", "狠");
            Add("heng", "横", "恒", "哼");
            Add("hong", "红", "宏", "洪", "轰");
            Add("hou", "后", "候", "厚", "侯", "猴");
            Add("hu", "湖", "互", "户", "呼", "护", "虎");
            Add("hua", "话", "华", "化", "花", "画", "滑");
            Add("huai", "坏", "怀", "淮");
            Add("huan", "欢", "换", "环", "还", "缓", "幻");
            Add("huang", "黄", "皇", "晃", "荒", "慌");
            Add("hui", "会", "回", "灰", "挥", "辉", "毁");
            Add("hun", "混", "婚", "魂", "昏");
            Add("huo", "活", "火", "或", "伙", "获", "货");
            Add("ji", "几", "及", "机", "基", "集", "计", "记", "际", "继");
            Add("jia", "家", "加", "价", "假", "架", "甲", "驾");
            Add("jian", "见", "间", "件", "建", "检", "简", "减", "尖");
            Add("jiang", "将", "讲", "江", "降", "奖", "酱");
            Add("jiao", "叫", "教", "交", "角", "脚", "校", "觉");
            Add("jie", "接", "节", "界", "解", "结", "街", "借", "姐");
            Add("jin", "进", "近", "今", "金", "斤", "紧", "尽");
            Add("jing", "经", "精", "惊", "京", "静", "景", "净", "境");
            Add("jiong", "窘", "炯");
            Add("jiu", "就", "九", "酒", "旧", "救", "久", "究");
            Add("ju", "句", "据", "巨", "局", "具", "居", "剧", "举");
            Add("juan", "卷", "捐", "倦", "圈");
            Add("jue", "决", "绝", "觉", "角");
            Add("jun", "军", "均", "君", "俊");
            Add("ka", "卡", "咖");
            Add("kai", "开", "凯", "慨");
            Add("kan", "看", "砍", "堪");
            Add("kang", "康", "抗", "扛");
            Add("kao", "考", "靠", "烤");
            Add("ke", "可", "克", "刻", "客", "课", "颗", "科");
            Add("ken", "肯", "恳");
            Add("keng", "坑");
            Add("kong", "空", "孔", "恐", "控");
            Add("kou", "口", "扣", "寇");
            Add("ku", "苦", "库", "哭", "裤");
            Add("kua", "夸", "跨", "垮");
            Add("kuai", "快", "块", "筷", "脍");
            Add("kuan", "款", "宽");
            Add("kuang", "况", "矿", "狂", "框");
            Add("kui", "亏", "愧", "奎");
            Add("kun", "困", "捆", "昆");
            Add("kuo", "阔", "扩", "括");
            Add("la", "拉", "啦", "辣", "落");
            Add("lai", "来", "赖", "睐");
            Add("lan", "蓝", "兰", "懒", "览", "烂");
            Add("lang", "浪", "朗", "郎", "狼");
            Add("lao", "老", "劳", "牢", "姥");
            Add("le", "了", "乐", "勒");
            Add("lei", "类", "累", "泪", "雷");
            Add("leng", "冷", "棱");
            Add("li", "里", "理", "立", "力", "利", "历", "例", "离", "丽");
            Add("lian", "连", "联", "脸", "练", "恋", "怜");
            Add("liang", "两", "量", "凉", "良", "亮", "梁");
            Add("liao", "了", "料", "聊", "辽", "疗");
            Add("lie", "列", "烈", "劣", "猎");
            Add("lin", "林", "临", "淋", "邻");
            Add("ling", "令", "另", "零", "领", "灵", "铃");
            Add("liu", "六", "流", "留", "刘", "柳");
            Add("long", "龙", "隆", "弄", "笼");
            Add("lou", "楼", "漏", "陋", "搂");
            Add("lu", "路", "陆", "录", "露", "鲁", "卢");
            Add("luan", "乱", "卵", "峦");
            Add("lun", "论", "轮", "伦");
            Add("luo", "罗", "落", "络", "洛");
            Add("lv", "绿", "律", "旅", "虑", "率");
            Add("ma", "吗", "妈", "马", "骂", "麻");
            Add("mai", "买", "卖", "迈", "埋");
            Add("man", "慢", "满", "漫", "蛮");
            Add("mang", "忙", "盲", "芒");
            Add("mao", "毛", "猫", "帽", "冒", "贸");
            Add("me", "么");
            Add("mei", "没", "美", "每", "妹", "煤", "梅");
            Add("men", "们", "门", "闷");
            Add("meng", "梦", "猛", "蒙", "盟");
            Add("mi", "米", "密", "秘", "迷", "蜜");
            Add("mian", "面", "免", "勉", "棉");
            Add("miao", "秒", "苗", "妙", "庙");
            Add("mie", "灭");
            Add("min", "民", "敏", "悯");
            Add("ming", "名", "明", "命", "鸣");
            Add("mo", "莫", "没", "末", "摸", "墨", "模");
            Add("mou", "某", "谋");
            Add("mu", "目", "木", "母", "墓", "幕", "牧");
            Add("na", "那", "哪", "拿", "纳", "呐");
            Add("nai", "奶", "耐", "奈");
            Add("nan", "南", "难", "男");
            Add("nang", "囊");
            Add("nao", "脑", "闹", "恼");
            Add("ne", "呢", "讷");
            Add("nei", "内", "馁");
            Add("nen", "嫩");
            Add("neng", "能");
            Add("ni", "你", "拟", "泥", "逆", "倪");
            Add("nian", "年", "念", "粘");
            Add("niang", "娘", "酿");
            Add("niao", "鸟", "尿");
            Add("nie", "捏", "涅");
            Add("nin", "您");
            Add("ning", "宁", "凝", "拧");
            Add("niu", "牛", "纽", "扭");
            Add("nong", "农", "浓", "弄");
            Add("nu", "怒", "奴", "努");
            Add("nuan", "暖");
            Add("nuo", "诺", "挪", "懦");
            Add("nv", "女");
            Add("ou", "欧", "偶", "呕", "鸥");
            Add("pa", "怕", "爬", "帕");
            Add("pai", "排", "派", "拍", "牌");
            Add("pan", "盘", "判", "盼", "攀");
            Add("pang", "旁", "胖");
            Add("pao", "跑", "泡", "炮", "抛");
            Add("pei", "配", "陪", "赔", "培");
            Add("pen", "盆", "喷");
            Add("peng", "朋", "碰", "棚", "膨");
            Add("pi", "皮", "批", "匹", "披", "疲");
            Add("pian", "片", "篇", "偏", "骗");
            Add("piao", "票", "飘", "漂");
            Add("pie", "瞥", "撇");
            Add("pin", "品", "拼", "频", "贫");
            Add("ping", "平", "凭", "评", "瓶", "苹");
            Add("po", "破", "坡", "婆", "迫");
            Add("pou", "剖");
            Add("pu", "普", "铺", "扑", "仆");
            Add("qi", "起", "其", "气", "七", "期", "奇", "齐", "妻");
            Add("qia", "恰", "卡", "掐");
            Add("qian", "前", "钱", "千", "浅", "签", "欠");
            Add("qiang", "强", "枪", "抢", "墙");
            Add("qiao", "桥", "巧", "敲", "瞧", "悄");
            Add("qie", "切", "且", "怯");
            Add("qin", "亲", "琴", "勤", "侵");
            Add("qing", "情", "清", "请", "青", "轻", "晴");
            Add("qiong", "穷", "琼");
            Add("qiu", "求", "秋", "球", "丘");
            Add("qu", "去", "取", "区", "曲", "趣", "趋");
            Add("quan", "全", "权", "劝", "泉", "圈");
            Add("que", "确", "却", "缺", "雀");
            Add("qun", "群", "裙");
            Add("ran", "然", "燃", "染");
            Add("rang", "让", "嚷", "壤");
            Add("rao", "绕", "扰", "饶");
            Add("re", "热", "惹");
            Add("ren", "人", "认", "任", "忍", "刃");
            Add("reng", "仍", "扔");
            Add("ri", "日");
            Add("rong", "容", "荣", "融", "绒");
            Add("rou", "肉", "柔", "揉");
            Add("ru", "如", "入", "儒", "乳");
            Add("ruan", "软", "阮");
            Add("rui", "瑞", "锐");
            Add("run", "润");
            Add("ruo", "若", "弱");
            Add("sa", "撒", "洒", "萨");
            Add("sai", "赛", "塞");
            Add("san", "三", "散", "伞");
            Add("sang", "桑", "丧", "嗓");
            Add("sao", "扫", "骚", "嫂");
            Add("se", "色", "瑟", "涩");
            Add("sen", "森");
            Add("seng", "僧");
            Add("sha", "沙", "杀", "傻", "纱");
            Add("shai", "晒", "筛");
            Add("shan", "山", "闪", "善", "扇", "单");
            Add("shang", "上", "商", "伤", "尚", "赏");
            Add("shao", "少", "烧", "梢", "稍", "绍");
            Add("she", "社", "设", "摄", "舌", "舍", "射");
            Add("shen", "身", "什", "深", "神", "申", "审");
            Add("sheng", "生", "声", "省", "升", "胜", "圣");
            Add("shi", "是", "事", "时", "十", "实", "使", "世", "市", "识", "师");
            Add("shou", "手", "首", "收", "受", "寿", "守");
            Add("shu", "书", "数", "树", "术", "输", "熟", "属", "束");
            Add("shua", "刷", "耍");
            Add("shuai", "帅", "衰", "摔");
            Add("shuan", "栓", "拴");
            Add("shuang", "双", "霜", "爽");
            Add("shui", "水", "睡", "谁", "税", "说");
            Add("shun", "顺", "瞬");
            Add("shuo", "说", "硕", "朔");
            Add("si", "四", "死", "思", "司", "斯", "私", "丝");
            Add("song", "送", "松", "宋", "诵");
            Add("sou", "搜", "艘", "嗽");
            Add("su", "速", "素", "诉", "苏", "宿");
            Add("suan", "算", "酸", "蒜");
            Add("sui", "岁", "随", "碎", "虽");
            Add("sun", "孙", "损", "笋");
            Add("suo", "所", "锁", "索", "缩");
            Add("ta", "他", "她", "它", "踏", "塔");
            Add("tai", "太", "态", "泰", "台", "抬");
            Add("tan", "谈", "探", "弹", "叹", "坦");
            Add("tang", "堂", "糖", "躺", "唐", "汤");
            Add("tao", "套", "逃", "讨", "桃", "陶");
            Add("te", "特", "忒");
            Add("teng", "疼", "腾", "藤");
            Add("ti", "体", "提", "题", "替", "踢");
            Add("tian", "天", "田", "填", "添", "甜");
            Add("tiao", "条", "跳", "挑", "调");
            Add("tie", "铁", "贴", "帖");
            Add("ting", "听", "停", "厅", "挺", "庭");
            Add("tong", "同", "通", "统", "痛", "童", "铜");
            Add("tou", "头", "透", "投", "偷");
            Add("tu", "图", "土", "突", "涂", "兔");
            Add("tuan", "团", "推");
            Add("tui", "退", "推", "腿");
            Add("tun", "吞", "屯");
            Add("tuo", "脱", "托", "拖", "妥");
            Add("wa", "哇", "娃", "挖", "瓦");
            Add("wai", "外", "歪");
            Add("wan", "万", "玩", "完", "晚", "碗", "弯");
            Add("wang", "望", "往", "网", "王", "亡", "忘");
            Add("wei", "为", "位", "未", "卫", "危", "微", "味");
            Add("wen", "问", "文", "闻", "温", "稳");
            Add("weng", "翁", "嗡");
            Add("wo", "我", "窝", "握", "卧", "喔");
            Add("wu", "五", "无", "物", "武", "舞", "屋", "误", "吴");
            Add("xi", "西", "系", "细", "息", "喜", "吸", "洗", "戏");
            Add("xia", "下", "夏", "吓", "狭", "虾");
            Add("xian", "现", "先", "线", "显", "险", "限", "县");
            Add("xiang", "想", "向", "像", "相", "香", "乡", "箱");
            Add("xiao", "小", "笑", "消", "效", "校", "肖", "销");
            Add("xie", "些", "写", "谢", "协", "斜", "鞋", "携");
            Add("xin", "心", "新", "信", "辛", "欣");
            Add("xing", "行", "性", "形", "星", "幸", "姓", "兴");
            Add("xiong", "胸", "雄", "凶", "熊");
            Add("xiu", "修", "休", "秀", "袖");
            Add("xu", "需", "许", "续", "叙", "虚", "序");
            Add("xuan", "选", "宣", "悬", "旋");
            Add("xue", "学", "雪", "血", "靴");
            Add("xun", "训", "讯", "寻", "迅", "循");
            Add("ya", "呀", "鸭", "牙", "压", "雅", "亚");
            Add("yan", "言", "眼", "严", "演", "验", "研", "烟");
            Add("yang", "样", "养", "阳", "洋", "羊", "杨");
            Add("yao", "要", "摇", "药", "腰", "咬", "遥");
            Add("ye", "也", "夜", "业", "叶", "爷", "野");
            Add("yi", "一", "已", "以", "意", "义", "易", "医", "依", "仪");
            Add("yin", "因", "引", "银", "音", "印", "阴");
            Add("ying", "应", "影", "营", "硬", "迎", "赢", "英");
            Add("yo", "哟");
            Add("yong", "用", "永", "勇", "拥", "泳");
            Add("you", "有", "又", "由", "右", "油", "友", "游", "优");
            Add("yu", "与", "于", "语", "育", "雨", "余", "鱼", "遇", "预");
            Add("yuan", "远", "元", "原", "圆", "愿", "院", "员", "园");
            Add("yue", "月", "约", "阅", "越", "悦", "跃");
            Add("yun", "云", "运", "允", "匀", "晕");
            Add("za", "砸", "杂");
            Add("zai", "在", "再", "载", "灾", "仔");
            Add("zan", "赞", "暂", "咱");
            Add("zang", "脏", "葬", "藏");
            Add("zao", "早", "造", "糟", "遭", "噪");
            Add("ze", "则", "责", "择");
            Add("zei", "贼");
            Add("zen", "怎");
            Add("zeng", "增", "赠", "曾");
            Add("zha", "扎", "炸", "闸", "眨");
            Add("zhai", "摘", "宅", "窄", "债");
            Add("zhan", "站", "展", "占", "战", "粘");
            Add("zhang", "张", "长", "掌", "涨", "帐", "障");
            Add("zhao", "找", "照", "招", "朝", "召", "赵");
            Add("zhe", "这", "着", "者", "折", "遮");
            Add("zhen", "真", "针", "阵", "镇", "振", "震");
            Add("zheng", "正", "整", "政", "证", "争", "征", "挣");
            Add("zhi", "只", "之", "直", "知", "指", "支", "制", "治", "至", "志");
            Add("zhong", "中", "重", "种", "终", "众", "钟");
            Add("zhou", "周", "洲", "舟", "州", "宙");
            Add("zhu", "主", "住", "注", "著", "助", "猪", "竹", "祝");
            Add("zhua", "抓", "爪");
            Add("zhuai", "拽");
            Add("zhuan", "转", "专", "传", "赚", "砖");
            Add("zhuang", "装", "状", "撞", "壮", "庄");
            Add("zhui", "追", "坠", "缀");
            Add("zhun", "准", "谆");
            Add("zhuo", "着", "捉", "桌", "琢");
            Add("zi", "自", "字", "子", "资", "紫", "姿");
            Add("zong", "总", "从", "宗", "综");
            Add("zou", "走", "奏", "揍");
            Add("zu", "组", "足", "祖", "族", "阻");
            Add("zuan", "钻", "攥");
            Add("zui", "最", "嘴", "醉", "罪");
            Add("zun", "尊", "遵");
            Add("zuo", "做", "作", "坐", "左", "座");
        }

        private static void InitSimpToTrad()
        {
            string simp = "万与丑专业丛东丝丢两严丧个丰临为丽举么义乌乐乔习乡书买乱争于亏云亘亚产亩亲亵亸亿仅从仑仓仪们价众优伙会伛伞伟传伤伥伦伧伪伫体余佣佥侠侣侥侦侧侨侩侪侬俨俩俪俭债倾偬偻偾偿傥傧储傩儿兑兖党兰关兴兹养兽冁冈冲决况冷冻净凄凉凌减凑凛几凤凫凭凯凶出函刍划刘则刚创删别刬刭刽刿剀剂剐剑剥剧劝办务劢动励劲劳势勋勐勚匀匦匮区协单卖卢卫却严历厉压厌厍厕厢厦厨厩厮县参双发变叙叠台叶号叹叽吁吃各吆合吉吊同名后吐向吓吕吗君吝吞吟吠吡吣吻吹呀呃呆呈告呐呕员呛呜呢咛呦周咙咛咳咸品哄哑哔喉喘喝唤喧嘈嚼囊囔团园囵国围园囵图圆圣压坏垄壮场坂块坟坚坛垒培基堂堑塞墅境墙增墟墨墩壮声壳壹壶处备复夏够头夸夹夺奁奂奄奇奥女奴奶奸她好如妆妇妈妊妒妹妻妾娘娱娄娅娃婚婶婷嫖婴孀子孔字存孙孜孝孟季学孩孪宁它宅宇守安宋完宏宓宗官宙定宛宝实宠审客宣室宪害宴家容宽宾宿寂寄寅密富寒寝察寡寨寸对寻导寿封射将尉尊小少尔尖尘尚尝尤尢尸尺尾尿局屁居届屈屋尸屏屎展屑属层屠屡履属山岁屹岂岗岘岗岚岛岭峄岳岷峨峪峦峰岛崇崆崎崭崂崔崞崛峥峪崚崭嵘崴崽嵬嵚崭崭崭";
            string trad = "萬與醜專業叢東絲丟兩嚴喪個豐臨為麗舉麼義烏樂喬習鄉書買亂爭於虧雲亙亞產畝親褻嚲億僅從侖倉儀們價眾優夥會傴傘偉傳傷倀倫傖偽佇體餘傭僉俠侶僥偵側僑儈儕儂儼倆儷儉債傾傯僂儇償儻儐儲儺兒兌兗黨蘭關興茲養獸囅岡沖決況冷凍淨淒涼凌減湊凜幾鳳鳧憑凱兇出函芻劃劉則剛創刪別剗剄劊劌剴劑剮劍剝劇勸辦務勱動勵勁勞勢勳猛勩勻匭匱區協單賣盧衛卻嚴歷厲壓厭厙廁廂廈廚廄廝縣參雙發變敘疊臺葉號嘆嘰吁吃各吆合吉吊同名後吐向嚇呂嗎君吝吞吟吠吡吣吻吹呀呃呆呈告吶嘔員嗆嗚呢嚀呦周嚨嚀咳鹹品哄啞嗶喉喘喝喚喧嘈嚼囊囔團園圇國圍園圇圖圓聖壓壞壟壯場坂塊墳堅壇壘培基堂塹塞墅境牆增墟墨墩壯聲殼壹壺處備復夏夠頭誇夾奪奩奐奄奇奧女奴奶姦她好如妝婦媽妊妒妹妻妾娘娛婁婭娃婚嬸婷嫖嬰孀子孔字存孫孜孝孟季學孩孿寧它宅宇守安宋完宏宓宗官宙定宛寶實寵審客宣室憲害宴家容寬賓宿寂寄寅密富寒寢察寡寨寸對尋導壽封射將尉尊小少爾尖塵尚嘗尤尢尸尺尾尿局屁居屆屈屋尸屏屎展屑屬層屠屢履屬山歲屹豈崗峴崗嵐島嶺嶧嶽岷峨峪巒峰島崇崆崎嶄嶗崔崞崛崢峪崚嶄嶸崴崽嵬嶔嶄嶄嶄";

            int len = Math.Min(simp.Length, trad.Length);
            for (int i = 0; i < len; i++)
            {
                if (!SimpToTrad.ContainsKey(simp[i]))
                {
                    SimpToTrad[simp[i]] = trad[i];
                }
            }
        }
    }
}
