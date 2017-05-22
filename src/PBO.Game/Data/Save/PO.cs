using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;

namespace PokemonBattleOnline.Game
{
    /// <summary>
    /// PO Pokemon
    /// </summary>
    [Serializable]
    public class Pokemon
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [XmlAttribute("Nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// 全国编号
        /// </summary>
        [XmlAttribute("Num")]
        public int Num { get; set; }

        /// <summary>
        /// 闪光
        /// </summary>
        [XmlAttribute("Shiny")]
        public int Shiny { get; set; }

        /// <summary>
        /// 特性
        /// </summary>
        [XmlAttribute("Ability")]
        public int Ability { get; set; }

        /// <summary>
        /// 世代
        /// </summary>
        [XmlAttribute("Gen")]
        public int Gen { get; set; }

        /// <summary>
        /// 道具
        /// </summary>
        [XmlAttribute("Item")]
        public int Item { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [XmlAttribute("Gender")]
        public int Gender { get; set; }

        /// <summary>
        /// 世代
        /// </summary>
        [XmlAttribute("SubGen")]
        public int SubGen { get; set; }

        /// <summary>
        /// 亲密度
        /// </summary>
        [XmlAttribute("Hapiness")]
        public int Hapiness { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [XmlAttribute("Lvl")]
        public int Lvl { get; set; }

        /// <summary>
        /// 性格
        /// </summary>
        [XmlAttribute("Nature")]
        public int Nature { get; set; }

        /// <summary>
        /// 形态
        /// </summary>
        [XmlAttribute("Forme")]
        public int Forme { get; set; }

        /// <summary>
        /// 技能
        /// </summary>
        [XmlElement("Move")]
        public int[] Move { get; set; }

        /// <summary>
        /// Iv
        /// </summary>
        [XmlElement("DV")]
        public int[] DV { get; set; }

        /// <summary>
        /// Ev
        /// </summary>
        [XmlElement("EV")]
        public int[] EV { get; set; }
    }

    /// <summary>
    /// PO Team
    /// </summary>
    [Serializable]
    public class Team
    {
        [XmlAttribute("gen")]
        public string Gen { get; set; }

        [XmlAttribute("subgen")]
        public string Subgen { get; set; }

        [XmlAttribute("defaultTier")]
        public string DefaulTtier { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement("Pokemon")]
        public Pokemon[] Pokemons { get; set; }
    }

    public static class TeamHelper
    {
        #region 0791

        private const string KEY = "sJg/jtph3lzuhiy9EafZtw==";
        private const string IV = "k/SHDmKCLIo=";

        /// <summary>
        /// 解密文件流
        /// </summary>
        private static MemoryStream Decrypt(FileStream stream)
        {
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create("RC2");
            algorithm.Key = Convert.FromBase64String(KEY);
            algorithm.IV = Convert.FromBase64String(IV);
            byte[] buffer = new byte[2050];
            MemoryStream ms = new MemoryStream();
            if (stream.Length != 0)
            {
                using (CryptoStream cs = new CryptoStream(stream, algorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    int num = 0;
                    do
                    {
                        num = cs.Read(buffer, 0, buffer.Length);
                        ms.Write(buffer, 0, num);
                    } while (num > 0);
                }
            }
            return ms;
        }

        private static readonly int[,] ABILITY_ID = { { 65, 0 }, { 65, 0 }, { 65, 0 }, { 66, 0 }, { 66, 0 }, { 66, 0 }, { 67, 0 }, { 67, 0 }, { 67, 0 }, { 19, 0 }, { 61, 0 }, { 14, 0 }, { 19, 0 }, { 61, 0 }, { 68, 0 }, { 51, 77 }, { 51, 77 }, { 51, 77 }, { 62, 50 }, { 62, 50 }, { 51, 0 }, { 51, 0 }, { 22, 61 }, { 22, 61 }, { 9, 0 }, { 9, 0 }, { 8, 0 }, { 8, 0 }, { 38, 79 }, { 38, 79 }, { 38, 79 }, { 38, 79 }, { 38, 79 }, { 38, 79 }, { 56, 98 }, { 56, 98 }, { 18, 0 }, { 18, 0 }, { 56, 0 }, { 56, 0 }, { 39, 0 }, { 39, 0 }, { 34, 0 }, { 34, 0 }, { 34, 0 }, { 27, 87 }, { 27, 87 }, { 14, 110 }, { 19, 110 }, { 8, 71 }, { 8, 71 }, { 53, 101 }, { 7, 101 }, { 6, 13 }, { 6, 13 }, { 72, 83 }, { 72, 83 }, { 18, 22 }, { 18, 22 }, { 6, 11 }, { 6, 11 }, { 6, 11 }, { 28, 39 }, { 28, 39 }, { 28, 39 }, { 62, 99 }, { 62, 99 }, { 62, 99 }, { 34, 0 }, { 34, 0 }, { 34, 0 }, { 29, 64 }, { 29, 64 }, { 69, 5 }, { 69, 5 }, { 69, 5 }, { 18, 50 }, { 18, 50 }, { 12, 20 }, { 12, 20 }, { 42, 5 }, { 42, 5 }, { 51, 39 }, { 50, 48 }, { 50, 48 }, { 47, 93 }, { 47, 93 }, { 1, 60 }, { 1, 60 }, { 75, 92 }, { 75, 92 }, { 26, 0 }, { 26, 0 }, { 26, 0 }, { 69, 5 }, { 15, 108 }, { 15, 108 }, { 52, 75 }, { 52, 75 }, { 9, 43 }, { 9, 43 }, { 34, 0 }, { 34, 0 }, { 69, 31 }, { 69, 31 }, { 7, 120 }, { 51, 89 }, { 20, 12 }, { 26, 0 }, { 26, 0 }, { 69, 31 }, { 69, 31 }, { 32, 30 }, { 34, 102 }, { 48, 113 }, { 33, 97 }, { 38, 97 }, { 33, 41 }, { 33, 41 }, { 35, 30 }, { 35, 30 }, { 43, 111 }, { 68, 101 }, { 12, 108 }, { 9, 0 }, { 49, 0 }, { 52, 104 }, { 22, 83 }, { 33, 0 }, { 22, 0 }, { 11, 75 }, { 7, 0 }, { 50, 91 }, { 11, 0 }, { 10, 0 }, { 18, 0 }, { 36, 88 }, { 75, 33 }, { 75, 33 }, { 33, 4 }, { 33, 4 }, { 69, 46 }, { 47, 17 }, { 46, 0 }, { 46, 0 }, { 46, 0 }, { 61, 0 }, { 61, 0 }, { 39, 0 }, { 46, 0 }, { 28, 0 }, { 65, 0 }, { 65, 0 }, { 65, 0 }, { 66, 0 }, { 66, 0 }, { 66, 0 }, { 67, 0 }, { 67, 0 }, { 67, 0 }, { 51, 50 }, { 51, 50 }, { 51, 15 }, { 51, 15 }, { 68, 48 }, { 68, 48 }, { 68, 15 }, { 68, 15 }, { 39, 0 }, { 10, 35 }, { 10, 35 }, { 9, 0 }, { 56, 98 }, { 56, 0 }, { 32, 55 }, { 32, 55 }, { 28, 48 }, { 28, 48 }, { 9, 0 }, { 9, 0 }, { 9, 0 }, { 34, 0 }, { 37, 47 }, { 37, 47 }, { 69, 5 }, { 11, 6 }, { 34, 102 }, { 34, 102 }, { 34, 102 }, { 53, 50 }, { 34, 94 }, { 34, 94 }, { 14, 3 }, { 11, 6 }, { 11, 6 }, { 28, 0 }, { 28, 0 }, { 15, 105 }, { 12, 20 }, { 26, 0 }, { 26, 0 }, { 23, 0 }, { 39, 48 }, { 5, 0 }, { 5, 0 }, { 32, 50 }, { 52, 8 }, { 69, 5 }, { 22, 50 }, { 22, 95 }, { 33, 38 }, { 68, 101 }, { 5, 82 }, { 68, 62 }, { 51, 39 }, { 53, 95 }, { 62, 95 }, { 40, 49 }, { 40, 49 }, { 12, 81 }, { 12, 81 }, { 55, 30 }, { 55, 97 }, { 21, 97 }, { 72, 55 }, { 33, 11 }, { 51, 5 }, { 48, 18 }, { 48, 18 }, { 33, 97 }, { 53, 0 }, { 5, 0 }, { 36, 88 }, { 22, 119 }, { 20, 101 }, { 62, 80 }, { 22, 101 }, { 12, 108 }, { 9, 0 }, { 49, 0 }, { 47, 113 }, { 32, 30 }, { 46, 0 }, { 46, 0 }, { 46, 0 }, { 62, 0 }, { 61, 0 }, { 45, 0 }, { 46, 0 }, { 46, 0 }, { 30, 0 }, { 65, 0 }, { 65, 0 }, { 65, 0 }, { 66, 0 }, { 66, 0 }, { 66, 0 }, { 67, 0 }, { 67, 0 }, { 67, 0 }, { 50, 95 }, { 22, 95 }, { 53, 82 }, { 53, 82 }, { 19, 0 }, { 61, 0 }, { 68, 0 }, { 61, 0 }, { 19, 0 }, { 33, 44 }, { 33, 44 }, { 33, 44 }, { 34, 48 }, { 34, 48 }, { 34, 48 }, { 62, 0 }, { 62, 0 }, { 51, 0 }, { 51, 0 }, { 28, 36 }, { 28, 36 }, { 28, 36 }, { 33, 0 }, { 22, 0 }, { 27, 90 }, { 27, 90 }, { 54, 0 }, { 72, 0 }, { 54, 0 }, { 14, 0 }, { 3, 0 }, { 25, 0 }, { 43, 0 }, { 43, 0 }, { 43, 0 }, { 47, 62 }, { 47, 62 }, { 47, 37 }, { 5, 42 }, { 56, 96 }, { 56, 96 }, { 51, 100 }, { 52, 22 }, { 69, 5 }, { 69, 5 }, { 69, 5 }, { 74, 0 }, { 74, 0 }, { 9, 31 }, { 9, 31 }, { 57, 0 }, { 58, 0 }, { 35, 68 }, { 12, 110 }, { 30, 38 }, { 64, 60 }, { 64, 60 }, { 24, 0 }, { 24, 0 }, { 41, 12 }, { 41, 12 }, { 12, 86 }, { 40, 116 }, { 73, 0 }, { 47, 20 }, { 47, 20 }, { 20, 77 }, { 52, 71 }, { 26, 0 }, { 26, 0 }, { 8, 0 }, { 8, 0 }, { 30, 0 }, { 30, 0 }, { 17, 0 }, { 61, 0 }, { 26, 0 }, { 26, 0 }, { 12, 107 }, { 12, 107 }, { 52, 75 }, { 52, 75 }, { 26, 0 }, { 26, 0 }, { 21, 0 }, { 21, 0 }, { 4, 0 }, { 4, 0 }, { 33, 0 }, { 63, 0 }, { 59, 0 }, { 16, 0 }, { 15, 119 }, { 15, 119 }, { 26, 0 }, { 46, 0 }, { 34, 94 }, { 26, 0 }, { 46, 105 }, { 23, 0 }, { 39, 115 }, { 39, 115 }, { 47, 115 }, { 47, 115 }, { 47, 115 }, { 75, 0 }, { 33, 0 }, { 33, 0 }, { 33, 69 }, { 33, 0 }, { 69, 0 }, { 69, 0 }, { 22, 0 }, { 29, 0 }, { 29, 0 }, { 29, 0 }, { 29, 0 }, { 29, 0 }, { 29, 0 }, { 26, 0 }, { 26, 0 }, { 2, 0 }, { 70, 0 }, { 76, 0 }, { 32, 0 }, { 46, 0 }, { 46, 0 }, { 46, 0 }, { 46, 0 }, { 65, 0 }, { 65, 0 }, { 65, 0 }, { 66, 0 }, { 66, 0 }, { 66, 0 }, { 67, 0 }, { 67, 0 }, { 67, 0 }, { 51, 0 }, { 22, 0 }, { 22, 0 }, { 86, 109 }, { 86, 109 }, { 61, 0 }, { 68, 0 }, { 79, 22 }, { 79, 22 }, { 79, 22 }, { 30, 38 }, { 30, 38 }, { 104, 0 }, { 104, 0 }, { 5, 0 }, { 5, 0 }, { 61, 0 }, { 107, 0 }, { 68, 0 }, { 118, 0 }, { 46, 0 }, { 50, 53 }, { 33, 0 }, { 33, 0 }, { 34, 0 }, { 122, 0 }, { 60, 114 }, { 60, 114 }, { 101, 53 }, { 106, 84 }, { 106, 84 }, { 50, 103 }, { 56, 103 }, { 26, 0 }, { 15, 105 }, { 7, 20 }, { 47, 20 }, { 26, 0 }, { 1, 106 }, { 1, 106 }, { 26, 85 }, { 26, 85 }, { 69, 5 }, { 43, 111 }, { 30, 32 }, { 51, 77 }, { 46, 0 }, { 8, 0 }, { 8, 0 }, { 8, 0 }, { 53, 47 }, { 80, 39 }, { 80, 39 }, { 45, 0 }, { 45, 0 }, { 4, 97 }, { 4, 97 }, { 107, 87 }, { 107, 87 }, { 26, 0 }, { 33, 114 }, { 33, 114 }, { 33, 11 }, { 117, 0 }, { 117, 0 }, { 46, 0 }, { 42, 5 }, { 12, 20 }, { 31, 116 }, { 34, 102 }, { 78, 0 }, { 49, 0 }, { 55, 32 }, { 3, 110 }, { 102, 0 }, { 81, 0 }, { 52, 8 }, { 12, 81 }, { 91, 88 }, { 80, 0 }, { 42, 5 }, { 46, 0 }, { 81, 0 }, { 26, 0 }, { 26, 0 }, { 26, 0 }, { 26, 0 }, { 46, 0 }, { 46, 0 }, { 18, 0 }, { 112, 0 }, { 46, 0 }, { 26, 0 }, { 93, 0 }, { 93, 0 }, { 123, 0 }, { 30, 0 }, { 121, 0 }, { 32, 0 }, { 26, 0 }, { 26, 0 }, { 26, 0 }, { 26, 0 }, { 26, 0 }, { 26, 0 } };
        private static int GetAbilityId(int identity, byte trait)
        {
            if (ABILITY_ID[identity - 1, 1] == 0) return ABILITY_ID[identity - 1, 0];
            return trait == 0 ? ABILITY_ID[identity - 1, 0] : ABILITY_ID[identity - 1, 1];
        }

        private static readonly int[] ITEM_IDS = { 0, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 80, 83, 81, 82, 86, 85, 87, 90, 75, 76, 78, 77, 84, 79, 88, 89, 61, 62, 60, 59, 23, 24, 11, 31, 21, 34, 28, 27, 29, 20, 30, 25, 26, 22, 17, 32, 33, 2, 3, 51, 19, 39, 38, 12, 13, 14, 40, 41, 64, 9, 74, 43, 44, 45, 16, 96, 7, 56, 93, 53, 42, 4, 47, 46, 63, 48, 54, 55, 18, 73, 5, 35, 52, 15, 50, 49, 58, 57, 10, 97, 72, 65, 6, 8, 1 };
        private static int GetItemId(int oldId)
        {
            return oldId > 149 ? 0 : ITEM_IDS[oldId];
        }

        private static readonly string[] MOVE_STRING = { "拍打", "手刀", "连环掌", "连续拳击", "百万拳击", "招财猫", "火焰拳", "冷冻拳", "雷电拳", "抓挠", "夹击", "断头台", "镰鼬", "舞剑", "居合斩", "旋风", "翅膀拍打", "吹飞", "飞天", "束缚", "打翻", "藤鞭", "践踏", "二段踢", "百万飞踢", "飞踢", "旋风腿", "泼沙", "头撞", "尖角突", "乱针", "尖角钻", "撞击", "压制", "卷紧", "突进", "暴走", "舍身撞", "摆尾", "毒针", "双针", "飞弹针", "瞪眼", "啃咬", "叫声", "吼叫", "唱歌", "超声波", "音速爆破", "残废", "溶解液", "火苗", "火焰喷射", "白雾", "水枪", "水压", "冲浪", "冷冻光", "暴风雪", "幻象光线", "泡沫光线", "北极光", "破坏光线", "啄击", "钻孔啄", "地狱车", "过肩摔", "反击", "地球投", "怪力", "吸收", "百万吸收", "寄生种子", "成长", "飞叶快刀", "阳光烈焰", "毒粉", "麻痹粉", "催眠粉", "花瓣舞", "吐丝", "龙之怒", "火焰漩涡", "电气震", "十万伏特", "电磁波", "雷电", "落石", "地震", "地裂", "挖洞", "剧毒", "念力", "精神干扰", "催眠术", "瑜珈姿势", "高速移动", "电光石火", "愤怒", "瞬间转移", "夜影", "模仿", "噪音", "影分身", "自我再生", "变硬", "变小", "烟幕", "怪异光", "缩头", "变圆", "栅栏", "光之壁", "黑雾", "反射盾", "集气", "克制", "摇手指", "鹦鹉学舌", "自爆", "蛋蛋爆弹", "舌头舔", "毒雾", "淤泥攻击", "骨头棒", "大字火", "攀瀑", "贝壳夹击", "迅星", "火箭头槌", "尖刺加农", "缠绕", "健忘", "念力弯匙", "生蛋", "飞膝踢", "蛇凝视", "食梦", "毒气", "投弹", "吸血", "恶魔之吻", "神鸟", "变身", "泡沫", "飘飘拳", "蘑菇孢子", "闪光", "精神波动", "鲤鱼翻身", "溶化", "螃蟹锤", "大爆炸", "乱抓", "骨头镖", "睡眠", "岩崩", "必杀门牙", "起棱角", "属性切换", "三角攻击", "愤怒门牙", "切断", "替身", "绝境反击", "素描", "三连踢", "偷窃", "蜘蛛网", "心眼", "恶梦", "火焰车", "打鼾", "诅咒", "垂死挣扎", "属性切换2", "空中爆破", "棉花孢子", "起死回生", "恨", "细雪", "保护", "音速拳", "恐惧颜", "偷袭", "天使之吻", "腹鼓", "毒爆弹", "泼泥", "喷墨", "撒菱", "电磁炮", "识破", "殊途同归", "灭亡歌", "冻结之风", "见切", "骨头闪", "锁定", "逆鳞", "沙尘暴", "亿万吸收", "忍耐", "撒娇", "滚动", "刀背打", "虚张声势", "饮奶", "电火花", "连续切", "钢之翼", "黑眼神", "着迷", "梦话", "治愈之铃", "报恩", "礼物", "撒气", "神秘守护", "痛平分", "神圣火焰", "震级变化", "爆裂拳", "百万大角", "龙之吐息", "接力棒", "鼓掌", "追击", "高速旋转", "香甜气息", "钢尾巴", "合金爪", "当身摔", "朝阳", "光合作用", "月光", "觉醒之力", "交叉突刺", "龙卷风", "祈雨", "放晴", "咬碎", "镜面反射", "自我暗示", "神速", "原始力量", "影球", "预知未来", "碎岩", "漩涡", "围攻", "下马威", "吵闹", "能量储存", "能量释放", "能量吸收", "热风", "冰雹", "寻衅", "煽动", "鬼火", "临别礼物", "空元气", "气合拳", "清醒", "跟我来", "自然力量", "充电", "挑拨", "帮手", "戏法", "复制", "许愿", "猫手", "扎根", "蛮力", "魔法外衣", "道具回收", "复仇", "瓦割", "哈欠", "打落", "莽撞", "喷火", "特性交换", "封印", "精神恢复", "怨念", "抢夺", "秘密之力", "潜水", "突张", "保护色", "萤火", "光栅清洗", "迷雾球", "羽毛舞", "草裙舞", "火焰踢", "玩泥巴", "冰球", "针刺臂膀", "偷懒", "高音", "剧毒之牙", "破碎爪", "爆裂燃烧", "高压水炮", "彗星拳", "惊吓", "天气球", "芳香疗法", "假哭", "真空斩", "燃烧殆尽", "敏锐嗅觉", "岩石封", "银色之风", "金属声", "草笛", "挠痒", "宇宙力量", "喷水", "信号光", "暗影拳", "神通力", "升龙拳", "沙地狱", "绝对零度", "浊流", "种子机枪", "燕返", "冰柱针", "铁壁", "禁止通行", "嗥叫", "龙爪", "硬化植物", "巨大化", "飞跳", "泥浆喷射", "毒尾巴", "索要", "高压电击", "魔力之叶", "玩水", "冥想", "叶刃斩", "龙之舞", "岩石爆破", "电击波", "水之波动", "破灭愿望", "精神增压", "鸟栖", "重力", "奇迹之瞳", "清醒拍打", "重臂锤", "螺旋球", "治愈之愿", "潮水", "自然恩惠", "佯攻", "啄食", "顺风", "点穴", "金属爆破", "蜻蜓返", "近身拳击", "报复", "再度欺压", "扣押", "掷物", "念力挪移", "皇牌", "回复封印", "榨取", "力量欺骗", "胃液", "祈祷", "先取", "效仿", "力量交换", "防御交换", "责罚", "最终保留", "烦恼种子", "不意打", "毒菱", "心灵交换", "液体圈", "电磁浮游", "炽热推进", "发劲", "波导弹", "岩切", "毒突", "暗黑波动", "试刀", "液态尾巴", "种子爆弹", "空气切割", "十字剪", "虫鸣", "龙之波动", "潜龙", "力量宝石", "吸收拳", "真空波", "气合弹", "能量球", "战鸟", "大地之力", "地下交易", "亿万冲击", "阴谋", "色彩拳", "雪崩", "冰屑飞射", "影子爪", "雷之牙", "冰之牙", "火之牙", "暗影击", "泥浆爆弹", "念力切割", "思念头槌", "镜反射光", "光栅炮", "攀石", "除雾", "欺骗空间", "流星群", "放电", "喷烟", "绿叶风暴", "蛮力藤鞭", "岩石大炮", "毒十字", "毒尘喷射", "铁头槌", "磁力炸弹", "石刃", "诱惑", "石砾", "草绳结", "喋喋不休", "制裁之石", "蛀蚀", "充电光束", "木叶槌", "水柱喷射", "攻击指令", "防御指令", "回复指令", "诸刃头突", "双重攻击", "时之咆哮", "亚空切断", "祈愿之舞", "掐碎", "岩浆风暴", "黑洞", "闪耀之种", "妖风", "影遁" };
        private static int GetMoveId(string move)
        {
            if (string.IsNullOrEmpty(move)) return 0;
            for (int i = 0; i < MOVE_STRING.Length; i++)
            {
                if (move == MOVE_STRING[i]) return i + 1;
            }
            return 0;
        }

        private static PokemonData GetById(int identity)
        {
            int[,] indexes = { { 387, 386, 1 }, { 388, 386, 2 }, { 389, 386, 3 }, { 497, 492, 1 }, { 498, 487, 1 }, { 499, 479, 1 }, { 500, 479, 3 }, { 501, 479, 4 }, { 502, 479, 2 }, { 503, 479, 5 } };

            int number = identity, form = 0;
            if (identity >= 390 && identity <= 496)
            {
                number -= 3;
            }
            else
            {
                for (int i = 0; i < indexes.GetLength(0); i++)
                {
                    if (indexes[i, 0] == identity)
                    {
                        number = indexes[i, 1];
                        form = indexes[i, 2];
                        break;
                    }
                }
            }

            return new PokemonData(number, form);
        }

        private static PokemonData ReadData(BinaryReader reader)
        {
            PokemonData pm = null;
            int identity = reader.ReadInt32();
            if (identity > 0 && identity < 504)
            {
                pm = GetById(identity);
                pm.Name = reader.ReadString();
                pm.Lv = reader.ReadByte();

                pm.Ev.Atk = reader.ReadByte();
                pm.Ev.Def = reader.ReadByte();
                pm.Ev.Speed = reader.ReadByte();
                pm.Ev.SpAtk = reader.ReadByte();
                pm.Ev.SpDef = reader.ReadByte();
                pm.Ev.Hp = reader.ReadByte();

                pm.Iv.Atk = reader.ReadByte();
                pm.Iv.Def = reader.ReadByte();
                pm.Iv.Speed = reader.ReadByte();
                pm.Iv.SpAtk = reader.ReadByte();
                pm.Iv.SpDef = reader.ReadByte();
                pm.Iv.Hp = reader.ReadByte();

                pm.Gender = (PokemonGender)reader.ReadInt32();
                int abilityId = GetAbilityId(identity, reader.ReadByte());

                pm.Ability = abilityId;
                pm.Nature = (PokemonNature)reader.ReadInt32();
                pm.Item = GetItemId(reader.ReadInt32());

                for (int i = 0; i < 4; i++)
                {
                    int moveId = GetMoveId(reader.ReadString());
                    if (moveId > 0)
                    {
                        MoveType move = RomData.GetMove(moveId);
                        if (move != null) pm.AddMove(move);
                    }
                }
            }
            return pm;
        }

        private static IEnumerable<PokemonData> From0791(FileStream stream)
        {
            using (MemoryStream ms = Decrypt(stream))
            {
                ms.Seek(0, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(ms);
                string name = reader.ReadString();
                string hash = reader.ReadString();

                for (int i = 0; i < 6; i++)
                {
                    var pm = ReadData(reader);
                    if (pm != null)
                    {
                        yield return pm;
                    }
                }
            }
        }

        public static PokemonTeam From0791(string path)
        {
            PokemonData[] pokemons;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                pokemons = From0791(fs).ToArray();
                fs.Close();
            }
            var team = new PokemonTeam(pokemons);
            team.Name = Path.GetFileNameWithoutExtension(path);
            return team;
        }

        #endregion

        #region pokemon online

        private static readonly StatType[] PO_STATS = { StatType.Hp, StatType.Atk, StatType.Def, StatType.SpAtk, StatType.SpDef, StatType.Speed };

        public static PokemonTeam FromPO(string path)
        {
            var team = ReadPOFile(path);
            return ToPokemonTeam(team);
        }

        public static void WriteToFile(string path, PokemonTeam pt)
        {
            var team = new Team();
            team.Pokemons = pt.Pokemons.Select(ToPokemon).ToArray();
            try
            {
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    var xs = new XmlSerializer(typeof(Team));
                    xs.Serialize(fs, team);
                    fs.Close();
                }
            }
            catch { }
        }

        private static Team ReadPOFile(string path)
        {
            Team team = null;
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    var xs = new XmlSerializer(typeof(Team));
                    team = (Team)xs.Deserialize(fs);
                    fs.Close();
                }
            }
            catch { }
            return team;
        }

        private static PokemonTeam ToPokemonTeam(Team team)
        {
            if (team != null)
            {
                var pokemons = team.Pokemons.Select(ToPokemonData).ToArray();
                return new PokemonTeam(pokemons);
            }
            return new PokemonTeam();
        }

        private static PokemonData ToPokemonData(Pokemon pm)
        {
            var data = new PokemonData(pm.Num, pm.Forme);

            data.Name = pm.Nickname;
            data.Ability = pm.Ability;
            data.Item = Item2PBO(pm.Item);
            data.Gender = (PokemonGender)pm.Gender;
            data.Nature = (PokemonNature)pm.Nature;
            data.Lv = pm.Lvl;
            data.Happiness = pm.Hapiness;

            for (int i = 0; i < 4 && i < pm.Move.Length; i++)
            {
                if (pm.Move[i] > 0)
                {
                    MoveType move = RomData.GetMove(pm.Move[i]);
                    if (move != null) data.AddMove(move);
                }
            }

            for (int i = 0; i < 6 && i < pm.DV.Length; i++)
            {
                data.Iv.SetStat(PO_STATS[i], pm.DV[i]);
            }

            for (int i = 0; i < 6 && i < pm.EV.Length; i++)
            {
                data.Ev.SetStat(PO_STATS[i], pm.EV[i]);
            }

            return data;
        }

        private static Pokemon ToPokemon(PokemonData data)
        {
            var pm = new Pokemon();
            pm.Num = data.Form.Species.Number;
            pm.Forme = data.Form.Index;
            pm.Nickname = data.Name;
            pm.Ability = data.Ability;
            pm.Item = Item2PO(data.Item);
            pm.Gender = (int)data.Gender;
            pm.Nature = (int)data.Nature;
            pm.Lvl = data.Lv;
            pm.Hapiness = data.Happiness;

            pm.Move = data.Moves.Select(m => m.Move.Id).ToArray();
            pm.DV = Get6D(data.Iv).ToArray();
            pm.EV = Get6D(data.Ev).ToArray();

            return pm;
        }

        private static IEnumerable<int> Get6D(I6D stat)
        {
            for (int i = 0; i < PO_STATS.Length; i++)
            {
                yield return stat.GetStat(PO_STATS[i]);
            }
        }

        private static readonly int[] PO_ITEMS = { 213, 159, 162, 3, 37, 163, 180, 17, 4, 87, 32, 131, 184, 60, 9, 125, 101, 15, 92, 34, 206, 103, 51, 48, 95, 106, 158, 166, 107, 132, 142, 57, 165, 31, 126, 29, 14, 156, 18, 200, 164, 38, 19, 39, 8, 93, 91, 22, 141, 71, 24, 10, 41, 102, 212, 13, 7, 50, 155, 33, 161, 160, 190, 5, 183, 170, 169, 171, 168, 167, 172, 30, 1, 6, 189, 197, 202, 194, 191, 188, 201, 187, 196, 195, 192, 199, 198, 185, 186, 193, 20, 27, 11, 36, 28, 181, 118, 227, 228, 229, 230, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 8000, 8001, 8002, 8003, 8004, 8005, 8006, 8007, 8008, 8009, 8010, 8011, 8012, 8013, 8014, 8015, 8016, 8017, 8018, 8019, 8020, 8021, 8022, 8023, 8024, 8025, 8026, 8027, 8028, 8029, 8030, 8031, 8032, 8033, 8034, 8035, 8036, 8037, 8038, 8039, 8040, 8041, 8042, 8043, 8044, 8045, 8046, 8047, 8048, 8049, 8050, 8051, 8052, 8053, 8054, 8055, 8056, 8057, 8058, 8059, 8060, 8061, 8062, 8063, 214, 45 };

        private static int Item2PO(int itemId)
        {
            if (itemId > 0 && itemId <= PO_ITEMS.Length) return PO_ITEMS[itemId - 1];
            return 0;
        }

        private static int Item2PBO(int itemId)
        {
            for (int i = 0; i < PO_ITEMS.Length; i++)
            {
                if (PO_ITEMS[i] == itemId) return i + 1;
            }
            return 0;
        }

        #endregion
    }
}