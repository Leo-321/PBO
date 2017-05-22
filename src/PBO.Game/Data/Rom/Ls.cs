namespace PokemonBattleOnline.Game
{
    public static class Ls
    {
        public const string SendOut1 = "SendOut1";
        public const string SendOut2 = "SendOut2";
        public const string SendOut22 = "SendOut22";
        public const string SendOut3 = "SendOut3";

        /// <summary>
        /// {0}强制退出了游戏。
        /// </summary>
        public const string PLAYER_STOP = "PLAYER_STOP";
        /// <summary>
        /// {0}断线了，游戏中止。
        /// </summary>
        public const string PLAYER_DISCONNECT = "PLAYER_DISCONNECT";
        /// <summary>
        /// {0}给精灵下达了错误的命令，游戏中止。
        /// </summary>
        public const string INVALID_INPUT = "INVALID_INPUT";
        /// <summary>
        /// 无法匹配主机传送过来的战报，战报关键字为{0}。
        /// </summary>
        public const string NO_KEY = "NO_KEY";
        /// <summary>
        /// 主机发生了错误，游戏中止。
        /// </summary>
        public const string SERROR = "SERROR";
        /// <summary>
        /// 客户端发生了错误，请退出对战窗口，必要时重启客户端。
        /// </summary>
        public const string CERROR = "CERROR";

        public const string PSN = "PSN"; //{0:p}受到了毒的伤害！
        public const string BRN = "BRN"; //{0:p}受到了烧伤的伤害！
        public const string Nightmare = "Nightmare";//{0:p}正在做恶梦！
        public const string TrapHurt = "TrapHurt"; //{0:p}受到了{1:m}的伤害！
        public const string ReHurt = "ReHurt"; //{0:p}因为反作用受到了伤害
        public const string HpRecover = "HpRecover"; //{0:p}回复了Hp
        public const string SandstormHurt = "SandstormHurt"; //沙尘暴袭击了{0:p}！
        public const string HailstormHurt = "HailstormHurt"; //冰雹袭击了{0:p}！
        public const string SetAbility = "SetAbility"; //{0:p}的特性变成了{1:a}！
        public const string RockyHelmet = "RockyHelmet"; //{0:p}因为凹凸头盔受到伤害！
        public const string PoisonHeal = "PoisonHeal"; //{0:p}吸收了毒液！
        public const string BadDreams = "BadDreams"; //{0:p}被噩梦魇住了！
        public const string AquaRing = "AquaRing"; //水之环回复了{0:p}的体力！
        public const string LeechSeed = "LeechSeed"; //种子夺走了{0:p}的体力！
        public const string BellyDrum = "BellyDrum"; //{0:p}消减体力、使出了全力！
        public const string Rest = "Rest"; //{0:p}睡着了，恢复了精神！
        public const string Wish = "Wish"; //{0:p}的许愿实现了！
        public const string Ingrain = "Ingrain"; //{0:p}从根吸取了养分！
        public const string EnSubstitute = "EnSubstitute"; //{0:p}的替身出现了！
        public const string Spikes = "Spikes"; //{0:p}受到了撒菱的伤害！
        public const string StealthRock = "StealthRock"; //{0:p}被尖锐的岩石扎伤了！
        public const string HpAbsorb = "HpAbosorb"; //从{0:p}那里吸取了体力！
        public const string FailSelfHurt = "FailSelfHurt"; //因为势头太猛，{0:p}撞到了地上！
        public const string ItemHpRecover = "ItemHpRecover"; //{0:p}使用{1:i}回复了体力！
        public const string ItemHpRecover2 = "ItemHpRecover2"; //{0:p}使用{1:i}回复了一点！
        public const string LifeOrb = "LifeOrb"; //{0:p}的生命被消减了一点！
        public const string ReHurtItem = "ReHurtItem"; //{0:p}因为{1:p}的{2:i}受到了伤害！
        public const string ItemHurt = "ItemHurt"; //{0:p}因为{1:i}受到了伤害！
        public const string EnCurse = "EnCurse"; //{0:p}消减了自己的体力，向{1:p}下了诅咒！
        public const string Curse = "Curse"; //{0:p}被诅咒了！
        public const string FlameBurst = "FlameBurst"; //火花降到了{0:p}的头上！
        public const string FireSea = "FireSea"; //{0:p}受到了火海的伤害！
        public const string ConfuseWork = "ConfuseWork"; //不知为什么攻击了自己！
        public const string Hurt = "Hurt"; //{0:p}受到了伤害
        public const string Powder = "Powder"; //火焰触碰到了粉尘，爆炸了！
        public const string Ability = "Ability";//{0:p}的{1:a}——

        public const string br = "br"; //
        public const string setAbility = "setAbility"; //（原特性：{0:a}）
        public const string php = "+hp"; //（+{0}）
        public const string nhp = "-hp"; //（{0}）
        public const string skillSwap = "skillSwap"; //（{0:p}的特性变成了{1:a}）
        public const string forceWithdraw = "forceWithdraw"; //（{0:p}强制下场）
        public const string painSplit = "painSplit"; //（双方的HP变成了{0}）

        //Withdraw:{0:pm.Owner.Name}收回了{0:pm.Name}！
        //Hits:命中{0}次！
        //Faint:{0:p}倒下了！
        //SuperHurt1:对{0:p}效果拔群！
        //SuperHurt2:对{0:p}和{1:p}效果拔群！
        //SuperHurt3:对{0:p}、{1:p}和{2:p}效果拔群！
        //WeakHurt1:对{0:p}没有什么效果。
        //WeakHurt2:对{0:p}和{1:p}没有什么效果。
        //WeakHurt3:对{0:p}、{1:p}和{2:p}没有什么效果。
        //Fail:但是对{0:p}没能成功！
        //7DUp1:{0:p}的{1:s}提升了！
        //7DUp2:{0:p}的{1:s}大幅提升！
        //7DUp3:{0:p}的{1:s}疯狂提升！
        //7DDown1:{0:p}的{1:s}下降了！
        //7DDown2:{0:p}的{1:s}大幅下降！
        //7DDown3:{0:p}的{1:s}疯狂下降！
        //7DMax:{0:p}的{1:s}不能再提升了！
        //7DMin:{0:p}的{1:s}不能再下降了！
        //7DReset:{0:p}的能力值恢复正常了！
        //7DLockAll:{0:p}的能力不能下降！
        //7DLock:{0:p}的{1:s}不能下降！
        //NoEffect:对{0:p}完全没有效果...
        /// <summary>
        /// 没有打中{0:p}！
        /// </summary>
        public const string Miss = "Miss";
        //Antiberry:{1:i}下降了给予{0:p}的伤害！
        //FormChange:{0:p}的形态改变了！
        //TurnLeft:{0}的精灵向左旋转了！
        //TurnRight:{0}的精灵向右旋转了！
        //MoveCenter:{0:p}向正中间移动了！
        //EnPSN:{0:p}中毒了！
        //EnBadlyPSN:{0:p}中了剧毒！
        //ItemEnBadlyPSN:{0:p}因为{1:i}中了剧毒！
        //DePSN:{0:p}的毒状态彻底消失了！
        //BeenPSN:{0:p}已经中毒了！
        //CantPSN:{0:p}不会中毒！
        //EnBRN:{0:p}烧伤了！
        //ItemEnBRN:{0:p}因为{1:i}烧伤了！
        //DeBRN:{0:p}的烧伤治好了！
        //BeenBRN:{0:p}已经烧伤了！
        //CantBRN:{0:p}不会被烧伤！
        //EnPAR:{0:p}麻痹了！不容易出招了！
        //PARWork:{0:p}因麻痹而不能行动
        //DePAR:{0:p}解除了麻痹！
        //BeenPAR:{0:p}已经麻痹了！
        //CantPAR:{0:p}不会被麻痹！
        //EnFRZ:{0:p}被冻住了！
        //FRZ:{0:p}被冻住了，无法行动！
        //DeFRZ:{0:p}身上的冰溶化了！
        //BeenFRZ:{0:p}已经被冻住了！
        //CantFRZ:{0:p}不会被冰冻！
        //DeFRZ2:{0:p}的{1:m}溶化了冰！
        //EnSLP:{0:p}睡着了！
        //SLP:{0:p}睡得很香...
        //DeSLP:{0:p}醒了过来！
        //BeenSLP:{0:p}已经睡着了！
        //CantSLP:{0:p}不会睡着！
        //EnNightmare:{0:p}开始做恶梦了！
        //EnAttract:{0:p}着迷了！
        //ItemEnAttract:{0:p}因为{1:i}着迷了！
        //Attract:{0:p}迷上了{1:p}！
        //AttractWork:{0:p}因为着迷而无法行动
        //DeAttract:{0:p}的着迷解除了！
        //StateReset:{0:p}的状态恢复正常了
        //Confuse:{0:p}混乱了！
        //DeConfuse:{0:p}的混乱解除了！
        //BeenConfuse:{0:p}已经混乱了！
        //CantConfuse:{0:p}不会混乱！
        //EnConfuse2:{0:p}因为疲劳至极混乱了！
        //Flinch:{0:p}因害怕而不能行动
        //DeFocusPunch:{0:p}失去了集中力，无法出招！
        //CanAttack:{0:p}的真实身份被识破了！
        //TrapFree:{0:p}被从{1:m}中解放了！
        //Trace:特性变成了{0:p}的{1:a}！
        //CT1:命中了{0:p}的要害！
        //CT2:命中了{0:p}和{1:p}的要害！
        //CT3:命中了{0:p}、{1:p}和{2:p}的要害！
        //PPRecover:{0:p}回复了{1:m}的PP
        //AllPPRecover:{0:p}的全部技能的PP都被回复了！
        //EnBalloon:{0:p}使用轻气球浮在空中！
        //DeBalloon:{0:p}的轻气球破了！
        //EjectButton:{0:p}使用逃生按钮回去了！
        //RedCard:{0:p}对{1:p}亮出了红牌！
        //FlashFire:{0:p}的火焰的威力提升了！
        //ReadMove:读取了{0:p}的{1:m}！
        //Anticipation:{0:p}在发抖！
        //Frisk:{0:p}看破了{1:i}！
        //MoldBreaker:{0:p}的破格！
        //Truant:{0:p}在偷懒……
        //ReTarget:{0:p}将攻击吸引了过来！
        //SuctionCups:{0:p}用吸盘紧紧地贴着！
        //Pickpocket:夺走了{0:p}的{1:i}！
        //MagicBounce:把{0:p}的{1:m}弹了回去！
        //Telepathy:{0:p}不会受到己方的攻击！
        //PoisonTouch:使{0:p}中毒了！
        //Harvest:{0:p}收获了{1:i}！
        //DeIllusion:{0:p}的幻影解除了！
        //AngerPoint:{0:p}的攻击提升到极限了！
        //Pressure:{0:p}释放出了压力！
        //Pickup:{0:p}捡来了{1:i}！
        //StickyHold:无法夺取{0:p}的道具！
        //EnSlowStart:{0:p}的状态不能提升！
        //DeSlowStart:{0:p}的状态恢复正常了！
        //Teravolt:{0:p}释放出了反弹的光辉！
        //Turboblaze:{0:p}释放出了熊熊燃烧的火焰！
        //SkillSwap:{0:p}和{1:p}交换了特性！
        //EnEndure:{0:p}进入了忍耐的姿势！
        //Endure:{0:p}承受住了攻击！
        //DeProtect:打破了{0:p}的守护！
        //Feint:{0:p}上了佯攻的当！
        //Rage:{0:p}的愤怒力量正在提升！
        //Prepare13:{0:p}的周围卷起了空气漩涡！
        //Prepare19:{0:p}高高地飞了起来！
        //Prepare76:{0:p}吸收了光！
        //Prepare91:{0:p}钻入了地下！
        //Prepare130:{0:p}把头缩回去了！
        //Prepare143:{0:p}被强光笼罩！
        //Prepare291:{0:p}潜入了水中！
        //Prepare340:{0:p}一跃而起！
        //Prepare467:{0:p}突然消失了！
        //Prepare553:{0:p}被寒光笼罩！
        //Prepare554:{0:p}被寒气笼罩！
        //EnEncore:{0:p}被鼓掌了！
        //DeEncore:{0:p}的鼓掌状态解除了！
        //EnGastroAcid:{0:p}的特性不起作用了！
        //EnTaunt:{0:p}被挑拨了！
        //Taunt:{0:p}被挑拨不能使用{1:m}！
        //DeTaunt:{0:p}的挑拨解除了！
        //EnTorment:{0:p}被加上寻衅了！
        //DeTorment:{0:p}的寻衅效果消失了！
        //EnImprison:{0:p}封印了对手的技能！
        //Imprison:{0:p}被封印无法使用{1:m}！
        //EnDisable:将{0:p}的{1:m}封印了！
        //Disable:{0:p}因为被束缚，所以无法使出{1:m}！
        //DeDisable:{0:p}的束缚解除了！
        //EnAquaRing:水之环笼罩了{0:p}！
        //EnLeechSeed:在{0:p}身上种了种子！
        //EnFocusPunch:{0:p}的集中力提升了！
        //RolePlay:{0:p}拷贝了{1:p}的{2:a}！
        //EnDestinyBond:{0:p}想要和对方同归于尽！
        //DestinyBond:{0:p}和对方同归于尽了！
        //EnGrudge:{0:p}想要将怨恨施加给对方！
        //Grudge:{0:p}因为怨恨，PP耗尽了！
        //Spite:消减了{0:p}的{1:m}的{2}点PP！
        /// <summary>
        /// {0:p}变成了{1:p}！
        /// </summary>
        public const string Transform = "Transform";
        //LockOn:{0:p}瞄准了{1:p}！
        //EnMagnetRise:{0:p}利用电磁力浮起来了！
        //DeMagnetRise:{0:p}的电磁力消失了！
        //Charge:{0:p}开始充电了！
        //EnYawn:引诱了{0:p}进入梦乡！
        /// <summary>
        /// {0:p}成为了注目的对象！
        /// </summary>
        public const string EnFollowMe = "EnFollowMe";
        //HeartSwap:{0:p}和{1:p}交换了能力等级！
        //PowerSwap:{0:p}和{1:p}交换了攻击与特攻能力等级！
        //GuardSwap:{0:p}和{1:p}交换了防御与特防能力等级！
        //Trick:{0:p}将双方的道具交换了！
        //GetItem:{0:p}得到了{1:i}！
        //Mimic:{0:p}学会了{1:m}！
        //LunarDance:{0:p}被神秘的月光包住了！
        //HealingWish:治愈之愿传递给了{0:p}！
        //EnUproar:{0:p}开始吵闹了！
        //UproarDeSLP:{0:p}因为太吵所以醒了！
        //UproarCantSLP:但是{0:p}因为太吵无法入睡！
        //UproarCantSLP2:但是{0:p}正在吵闹，无法入睡！
        //Uproar:{0:p}正在吵闹！
        //DeUproar:{0:p}安静下来了！
        //EnStockpile:{0:p}积累了{1}次！
        //DeStockpile:{0:p}积累的效果消失了！
        //EnEmbargo:{0:p}不能使用道具了！
        //DeEmbargo:{0:p}可以使用道具了！
        //Recycle:{0:p}捡到了{1:i}！
        //EnIngrain:{0:p}扎下了根！
        //IngrainCantMove:{0:p}扎下了根，不能移动！
        //Bide:{0:p}正在克制自己……
        //DeBide:{0:p}把累积的愤怒释放了！
        //EnSnatch:{0:p}在观察对手的行动！
        //Snatch:{0:p}抢走了{1:p}的技能！
        //EnMagicCoat:{0:p}被魔装反射包住了！
        //MagicCoat:{0:p}将{1:m}反弹了回去！
        //SelfWithdraw:{0:pm.Name}回到了{0:pm.Owner.Name}身边！
        //PowerTrick:{0:p}的攻击和防御交换了！
        //EatDefenderBerry:{0:p}夺取了{1:i}，开始吃了！
        //Fling:{0:p}把{1:i}扔出去了！
        //HasSubstitute:但是{0:p}的替身已经出现了……
        //HurtSubstitute:{0:p}的替身承受了攻击！
        //DeSubstitute:{0:p}的替身消失了……
        //WideGuard:广域防御保护了{0:p}！
        //QuickGuard:快速防御保护了{0:p}！
        //EnTrap20:{0:p}被{1:p}缠住了！
        //EnTrap35:{0:p}被{1:p}缠上了！
        //EnTrap128:{0:p}被{1:p}的壳夹住了！
        //EnTrap250:{0:p}被困在漩涡里了！
        //EnTrap83:{0:p}被困在火焰的旋涡里了！
        //EnTrap463:{0:p}被困在熔岩的旋涡里了！
        //EnTrap328:{0:p}被困在沙地狱里了！
        //Struggle:{0:p}没有能够使用的技能！
        //Safeguard:{0:p}被神秘的纱幕保护住了！
        //Mist:{0:p}被白雾保护住了！
        //ForceSendOut:{0:pm.Owner.Name}的{0:pm.Name}（Lv.{0:pm.Lv} {0:pm.Form}）被拽出来战斗了！
        //Stiff:{0:p}因为攻击的反作用不能行动！
        //EnTrickRoom:{0:p}扭曲了时空！
        //PerishSong:{0:p}的毁灭的倒计时还剩下{1}！
        //EnCantSelectWithdraw:{0:p}已经逃不掉了！
        //EnHealBlock:{0:p}的回复动作被封印了！
        //DeHealBlock:{0:p}的回复封印解除了！
        //HealBlock:{0:p}因为回复封印无法回复!
        //HealBlockCantUseMove:{0:p}因为回复封印无法使用{1:m}!
        //FullHp:但是{0:p}体力充沛!
        //TypeChange:{0:p}变成了{1:b}属性！
        //FailSp:{0:p}不能使用{1:m}！
        //ItemPPRecover:{0:p}使用{1:i}回复了{2:m}的PP！
        //ItemDePSN:{0:p}使用{1:i}解了毒！
        //ItemDePAR:{0:p}使用{1:i}治愈了麻痹状态！
        //ItemDeSLP:{0:p}使用{1:i}醒了过来！
        //ItemDeFRZ:{0:p}使用{1:i}治愈了冰冻状态！
        //ItemDeBRN:{0:p}使用{1:i}治愈了烧伤状态！
        //ItemDeConfuse:{0:p}使用{1:i}治愈了混乱状态！
        //ItemDeAttract:{0:p}使用{1:i}治愈了着迷状态！
        //Item7DUp1:{0:p}使用{1:i}使{2:s}提升！
        //Item7DUp2:{0:p}使用{1:i}使{2:s}大幅提升！
        //Item7DUp3:{0:p}使用{1:i}使{2:s}疯狂提升！
        //ItemEnFocusEnergy:{0:p}使用{1:i}后，充满了干劲！
        //FocusItem:{0:p}使用{1:i}撑了下来！
        //WhiteHerb:{0:p}使用了{1:i}复原了所有能力值！
        /// <summary>
        /// {0:p}使用了{1:i}使全身充满了力量！
        /// </summary>
        public const string PowerHerb = "PowerHerb";
        //QuickItem:{0:p}使用了{1:i}使行动变快了！
        //MicleBerry:{0:p}使用了{1:i}使得下个使用的技能更容易命中了！
        //EnFocusEnergy:{0:p}的干劲十足！
        /// <summary>
        /// {0:p}准备帮助{1:p}！
        /// </summary>
        public const string HelpingHand = "HelpingHand";
        //PsychUp:{0:p}复制了{1:p}的能力变化！
        //KnockOff:{2:p}把{0:p}的{1:i}打掉了！
        //Thief:{0:p}从{2:p}那里夺走了{1:i}！
        //EnFSDD248:{0:p}预知了未来的攻击！
        //EnFSDD353:{0:p}将破灭之愿托付给了未来！
        //FSDD:{0:p}受到了{1:m}的攻击！
        //Gravity:{0:p}受到重力的影响，不能漂浮在空中了！
        //GravityCantUseMove:{0:p}因为重力太强，使不出{1:m}！
        //ReflectType:{0:p}的属性变成和{1:p}一样的了！
        //PowerSplit:{0:p}和{1:p}共享了力量！
        //GuardSplit:{0:p}和{1:p}共享了防御！
        //Autotomize:{0:p}身体变轻了！
        //Incinerate:{0:p}的{1:i}被烧没了！
        //Bestow:{0:p}从{2:p}那里获得了{1:i}！
        /// <summary>
        /// {0:p}把{1:p}带到空中去了！
        /// </summary>
        public const string EnSkyDrop = "EnSkyDrop";
        //DeSkyDrop:{0:p}从自由降落中解脱了！
        //EnSmackDown:{0:p}被击落到了地面上！
        /// <summary>
        /// {0:p}的顺序被推迟了！
        /// </summary>
        public const string Quash = "Quash";
        /// <summary>
        /// {0:p}听信了对手的话！
        /// </summary>
        public const string AfterYou = "AfterYou";
        //AllySwitch:{0:p}和{1:p}交换了位置！
        //EnTelekinesis:{0:p}漂浮在空中！
        //DeTelekinesis:{0:p}被从念动力中释放了！
        /// <summary>
        /// 但是没能成功！！
        /// </summary>
        public const string Fail0 = "Fail0";
        //SuperHurt0:效果拔群!
        //WeakHurt0:没有什么效果...
        //CT0:打中了要害！
        //NoPP:但是技能已经没有剩余的PP点数了！
        /// <summary>
        /// 阳光变强烈了！
        /// </summary>
        public const string EnIntenseSunlight = "EnIntenseSunlight";
        /// <summary>
        /// 开始下雨了！
        /// </summary>
        public const string EnRain = "EnRain";
        /// <summary>
        /// 开始刮沙尘暴了！
        /// </summary>
        public const string EnSandstorm = "EnSandstorm";
        /// <summary>
        /// 开始下冰雹了！
        /// </summary>
        public const string EnHailstorm = "EnHailstorm";
        /// <summary>
        /// 阳光恢复了原样！
        /// </summary>
        public const string DeIntenseSunlight = "DeIntenseSunlight";
        /// <summary>
        /// 雨停了！
        /// </summary>
        public const string DeRain = "DeRain";
        /// <summary>
        /// 沙尘暴停了！
        /// </summary>
        public const string DeSandstorm = "DeSandstorm";
        /// <summary>
        /// 冰雹停了！
        /// </summary>
        public const string DeHailstorm = "DeHailstorm";
        //AirLock:天气的影响消失了！
        //Sandstorm:沙尘暴刮得很凶！
        //Hailstorm:冰雹下得很凶！
        //OHKO:一击必杀！
        //Haze:全部的能力值都复原了！
        //Splash:但是什么都没有发生！
        //Magnitude:震级{0}！
        //HealBell:铃铛的声音响了起来！
        //Aromatherapy:清新的香气传播开来了！
        //PainSplit:{0:p}和{1:p}互相分担了体力！
        //DeTrickRoom:扭曲的时空恢复了原状！
        //EnGravity:重力变强了！
        //DeGravity:重力恢复了原状！
        //EnPerishSong:听到灭亡歌的精灵在3回合后将会毁灭！
        //Metronome:摇动手指后，使出了{1:m}！
        //NaturePower:自然力量变成了{1:m}！
        //PayDay:金币撒在了周围！
        //NoHp4Substitute:但是体力已经不够制造替身了！
        //EnReflect:反射盾使{0:t}的队伍的物理抗性提高了！
        //DeReflect:{0:t}的队伍的反射盾消失了！
        //EnLightScreen:光之壁使{0:t}的队伍的特殊抗性提高了！
        //DeLightScreen:{0:t}的队伍的光之壁消失了！
        //EnSafeguard:{0:t}的队伍被神秘的纱幕包住了！
        //DeSafeguard:包住{0:t}的队伍的神秘的纱幕消失了！
        //EnMist:{0:t}的队伍被白雾包住了！
        //DeMist:包住{0:t}的队伍的白雾消失了！
        //EnTailwind:顺风开始吹向{0:t}的队伍！
        //DeTailwind:吹向{0:t}的队伍的顺风停止了！
        //EnLuckyChant:祈祷的力量隐藏起了{0:t}的队伍的要害！
        //DeLuckyChant:{0:t}的队伍的祈祷解除了！
        //EnSpikes:{0:t}的队伍的脚下撒满了菱镖！
        //DeSpikes:{0:t}的队伍脚下的菱镖消失了！
        //EnToxicSpikes:{0:t}的队伍的脚下撒满了毒菱！
        //DeToxicSpikes:{0:t}的队伍脚下的毒菱消失了！
        //EnStealthRock:{0:t}的队伍周围开始出现尖锐的岩石！
        //DeStealthRock:{0:t}的队伍周围的隐蔽石砾消失了！
        //EnWideGuard:广域防御守护了{0:t}的队伍的周围！
        //EnQuickGuard:快速防御守护了{0:t}的队伍的周围！
        /// <summary>
        /// {0:t}的队伍上空出现了彩虹！
        /// </summary>
        public const string EnRainbow = "EnRainbow";
        //DeRainbow:彩虹从{0:t}的队伍上空消失了！
        /// <summary>
        /// 火海包围了{0:t}的队伍！
        /// </summary>
        public const string EnFireSea = "EnFireSea";
        //DeFireSea:包围{0:t}的队伍的火海消失了！
        /// <summary>
        /// {0:t}的队伍的周围出现了湿原野！
        /// </summary>
        public const string EnSwamp = "EnSwamp";
        //DeSwamp:{0:t}的队伍周围的的湿原野消失了！
        //Unnerve:{0:t}的队伍因为紧张而吃不下树果了！
        //EnWonderRoom:制造出了防御与特防互换的空间！
        //DeWonderRoom:奇迹空间被解除，防御与特防恢复了原样！
        //EnMagicRoom:制造出了使持有的道具消失的空间！
        //DeMagicRoom:魔法空间被解除，道具的效果恢复了原样！
        //Gem:{0:i}提升了{1:m}的威力！
        //Prepare566:{0:p}消失了！
        //Prepare601:{0:p}咬紧嘴唇！
        //EnTrap611:{0:p}被困在虫群里了！
        //EnZenMode:{0:p}的不倒翁模式发动！
        //DeZenMode:{0:p}的不倒翁模式解除！
        //Protect:{0:p}保护了自己！
        //EnProtect:{0:p}进入了守护的姿势！
        /// <summary>
        /// {0:p}等待着{1:p}的技能...
        /// </summary>
        public const string Pledge = "Pledge";
        /// <summary>
        /// 两个技能交汇了！成为了组合技！
        /// </summary>
        public const string Pledges = "Pledges";
        //subtitle_AssaultVest:[暂]因为{2}的效果不能使用变化技能！
        //EnMatBlock:{0:p}准备叠毯子阻止攻击！
        //MatBlock:踢出去的毯子阻止了{0:m}！
        //EnStickyWeb:{0:t}的场地被巨大的黏网覆盖！
        //StickyWeb:{0:p}被网缠住了！
        //DeStickyWeb:[暂]{0:t}脚下的黏网消失了！
        //EnPowder:{0:p}被粉尘覆盖！
        //AddType:{0:p}被施加了{1:b}属性！
        //EnGrassyTerrain:脚下长满了绿草！
        //DeGrassyTerrain:脚下的草已消失！
        //EnElectricTerrain:[暂]脚下出现了电流！
        //ElectricTerrain:[暂]{0:p}因为电流场地不能睡眠！
        //DeElectricTerrain:[暂]脚下的电流消失了！
        //EnMistyTerrain:[暂]场地出现了迷雾！
        //MistyTerrain:[暂]{0:p}被迷雾场地保护住了！
        //DeMistyTerrain:[暂]迷雾场地消失了！
        //EnFairyLock:[暂]下回合精灵不能交换！
        //TopsyTurvy:[暂]{0:p}的能力等级被逆转了！
        //EnCraftyShield:[暂]欺诈防御守护了{0:t}的队伍的周围！
        //CraftyShield:[暂]欺诈防御保护了{0:p}！
        //Magician:[暂]夺走了{0:p}的{1:i}！
        //EnWaterSport:火焰的威力减弱了！
        //DeWaterSport:玩水的效果消失了！
        //EnMudSport:电气的威力减弱了！
        //DeMudSport:玩泥的效果消失了！
        //EnIonDeluge:暴雨般的离子倾洒在场地上！
        //EnElectrify:{0:p}的技能带电了！
        //SweetVeil0:{0:p}将自己掩护于甜蜜之中！
        //SweetVeil1:{0:p}将{1:p}掩护于甜蜜之中！
        //StanceChangeSword:变成了剑形态！
        //StanceChangeShield:变成了盾形态！
        //WeaknessPolicy:[暂]{0:p}的弱点保护发动！
        //SafetyGoggles:[暂]{0:p}的防尘护目镜发动！
        //Celebrate:恭喜{0:pm.Owner.Name}！
        //MegaPre:{0:pm.Name}的{1:i}与{0:pm.Owner.Name}的键钥之石产生共鸣！
        //Mega:{0:p}超强进化成了{0:pm.Form}！
        /// <summary>
        /// {0:p}的原始回归！取回了原始的姿态！
        /// </summary>
        public const string Primal = "Primal";
        /// <summary>
        /// {0:pm.Owner.Name}的强烈祈祷向{0:pm.Name}传达——
        /// </summary>
        public const string MegaPre384 = "MegaPre384";
        /// <summary>
        /// 开始降强雨了！
        /// </summary>
        public const string EnHeavyRain = "EnHeavyRain";
        /// <summary>
        /// 受到强雨的影响火系技能的效果消失了！
        /// </summary>
        public const string HeavyRain = "HeavyRain";
        /// <summary>
        /// 强雨的势态没有停止！
        /// </summary>
        public const string HeavyRain2 = "HeavyRain2";
        /// <summary>
        /// 强雨停止了！
        /// </summary>
        public const string DeHeavyRain = "DeHeavyRain";
        /// <summary>
        /// 阳光变得非常强烈！
        /// </summary>
        public const string EnHarshSunlight = "EnHarshSunlight";
        /// <summary>
        /// 受到强烈阳光的影响水系技能被蒸发了！
        /// </summary>
        public const string HarshSunlight = "HarshSunlight";
        /// <summary>
        /// 强烈阳光没有消失！
        /// </summary>
        public const string HarshSunlight2 = "HarshSunlight2";
        /// <summary>
        /// 阳光复原了！
        /// </summary>
        public const string DeHarshSunlight = "DeHarshSunlight";
        /// <summary>
        /// 谜之乱气流保护着飞行精灵！
        /// </summary>
        public const string EnMysteriousAirCurrent = "EnMysteriousAirCurrent";
        /// <summary>
        /// 谜之乱气流减弱了攻击！
        /// </summary>
        public const string MysteriousAirCurrent = "MysteriousAirCurrent";
        /// <summary>
        /// 谜之乱气流没有停止！
        /// </summary>
        public const string MysteriousAirCurrent2 = "MysteriousAirCurrent2";
        /// <summary>
        /// 谜之乱气流消失了！
        /// </summary>
        public const string DeMysteriousAirCurrent = "DeMysteriousAirCurrent";
    }
}