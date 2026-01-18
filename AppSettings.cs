using System;

namespace BBMath.Configuration
{
    /// <summary>
    /// 强类型应用程序配置类
    /// </summary>
    public class AppSettings
    {
        private readonly IConfigurationService _configService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configService">配置服务实例</param>
        public AppSettings(IConfigurationService configService)
        {
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
        }

        /// <summary>
        /// 默认构造函数，使用默认的INI文件配置服务
        /// </summary>
        public AppSettings() : this(new IniConfigurationService())
        {
        }

        /// <summary>
        /// 金币总数
        /// </summary>
        public int CoinTotal
        {
            get => _configService.ReadInt("APP", "coinTtl", Core.AppConstants.DefaultCoinTotal);
            set => _configService.WriteValue("APP", "coinTtl", value.ToString());
        }

        /// <summary>
        /// 管理员密码
        /// </summary>
        public string Password
        {
            get => _configService.ReadValue("General", "PSW");
            set => _configService.WriteValue("General", "PSW", value);
        }

        /// <summary>
        /// 是否启用辅助框
        /// </summary>
        public bool HelpBox
        {
            get => _configService.ReadBool("APP", "helpBox");
            set => _configService.WriteValue("APP", "helpBox", value.ToString());
        }

        /// <summary>
        /// 辅助框是否免费使用
        /// </summary>
        public bool HelpBoxFree
        {
            get => _configService.ReadBool("APP", "helpBoxFree");
            set => _configService.WriteValue("APP", "helpBoxFree", value.ToString());
        }

        /// <summary>
        /// 暂停类型：0=限次数暂停，1=限时间暂停
        /// </summary>
        public int PauseType
        {
            get => _configService.ReadInt("APP", "pauseType");
            set => _configService.WriteValue("APP", "pauseType", value.ToString());
        }

        /// <summary>
        /// 剩余暂停时间（秒）
        /// </summary>
        public int PauseSecondsLeft
        {
            get => _configService.ReadInt("APP", "pauseSecLeft");
            set => _configService.WriteValue("APP", "pauseSecLeft", value.ToString());
        }

        /// <summary>
        /// 可用暂停次数
        /// </summary>
        public int AllowPauseCount
        {
            get => _configService.ReadInt("APP", "allowPause");
            set => _configService.WriteValue("APP", "allowPause", value.ToString());
        }

        /// <summary>
        /// 错误消息显示时间（秒）
        /// </summary>
        public int ErrorMessageShowTime
        {
            get => _configService.ReadInt("APP", "errorMsgShowTime");
            set => _configService.WriteValue("APP", "errorMsgShowTime", value.ToString());
        }

        /// <summary>
        /// 答对奖励金币数
        /// </summary>
        public int AwardCoinPerCorrect
        {
            get => _configService.ReadInt("APP", "awardCoin");
            set => _configService.WriteValue("APP", "awardCoin", value.ToString());
        }

        /// <summary>
        /// 检查辅助框花费金币
        /// </summary>
        public int CostCoinCheck
        {
            get => _configService.ReadInt("APP", "costCoinCheck");
            set => _configService.WriteValue("APP", "costCoinCheck", value.ToString());
        }

        /// <summary>
        /// 给出答案花费金币
        /// </summary>
        public int CostCoinGive
        {
            get => _configService.ReadInt("APP", "costCoinGive");
            set => _configService.WriteValue("APP", "costCoinGive", value.ToString());
        }

        /// <summary>
        /// 惩罚加题数量
        /// </summary>
        public int PunishmentAddQuestions
        {
            get => _configService.ReadInt("APP", "punishment");
            set => _configService.WriteValue("APP", "punishment", value.ToString());
        }

        /// <summary>
        /// 超时惩罚加题数量
        /// </summary>
        public int PunishmentTimeout
        {
            get => _configService.ReadInt("APP", "punishmentTimeOut");
            set => _configService.WriteValue("APP", "punishmentTimeOut", value.ToString());
        }

        /// <summary>
        /// 保存所有配置变更
        /// </summary>
        public void Save()
        {
            // INI配置服务的WriteValue已立即写入文件，此方法保留以备扩展
            // 例如：批量写入、事务性保存等
        }

        /// <summary>
        /// 重新加载配置
        /// </summary>
        public void Reload()
        {
            // 对于INI文件，重新读取文件需要重新初始化服务
            // 当前实现不支持热重载，此方法预留
        }
    }
}