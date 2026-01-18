using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BBMath.Core;
using BBMath.Configuration;

namespace BBMath.Tests
{
    [TestClass]
    public class GameStateManagerTests
    {
        private string _tempIniPath;
        
        [TestInitialize]
        public void TestInitialize()
        {
            // 创建临时INI文件路径
            _tempIniPath = Path.Combine(Path.GetTempPath(), "bbmath_test.cfg");
            
            // 重置 GameStateManager 静态字段为默认值
            ResetGameStateManager();
            
            // 设置临时配置文件路径（完整路径）
            GameStateManager.iniFileName = _tempIniPath;
            
            // 确保 AppSettings 使用临时文件
            // 通过反射重置 _appSettings 为 null，使其重新创建
            ResetAppSettingsInstance();
        }
        
        [TestCleanup]
        public void TestCleanup()
        {
            // 删除临时文件
            if (File.Exists(_tempIniPath))
            {
                try { File.Delete(_tempIniPath); } catch { }
            }
            
            // 恢复默认INI文件名
            GameStateManager.iniFileName = "bbmath.cfg";
        }
        
        private void ResetGameStateManager()
        {
            GameStateManager.lstExamObjects = null;
            GameStateManager.currentTypeIndex = 0;
            GameStateManager.finished = false;
            GameStateManager.examTtl = 15;
            GameStateManager.examTtlRec = 0;
            GameStateManager.punishment = 2;
            GameStateManager.correct = 0;
            GameStateManager.wrong = 0;
            GameStateManager.helpBox = true;
            GameStateManager.helpBoxFree = false;
            GameStateManager.coinTtl = 50;
            GameStateManager.costCoinCheck = 1;
            GameStateManager.costCoinGive = 3;
            GameStateManager.awardCoin = 3;
            GameStateManager.punishmentTimeOut = 1;
            GameStateManager.PSW = "qiwei";
            GameStateManager.flagPause = false;
            GameStateManager.allowPause = 5;
            GameStateManager.errorMsgShowTime = 5;
            GameStateManager.pauseType = 1;
            GameStateManager.pauseSecLeft = 600;
        }
        
        private void ResetAppSettingsInstance()
        {
            // 通过反射将 GameStateManager._appSettings 设置为 null
            var field = typeof(GameStateManager).GetField("_appSettings", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            if (field != null)
            {
                field.SetValue(null, null);
            }
        }
        
        [TestMethod]
        public void AwardCoins_ShouldIncreaseTotalCoin()
        {
            // Arrange
            int initialCoins = GameStateManager.coinTtl;
            int awardAmount = 10;
            
            // Act
            GameStateManager.AwardCoins(awardAmount);
            
            // Assert
            Assert.AreEqual(initialCoins + awardAmount, GameStateManager.coinTtl);
            Assert.AreEqual(initialCoins + awardAmount, GameStateManager.TotalCoin);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AwardCoins_ShouldThrowException_WhenNegativeAmount()
        {
            // Act
            GameStateManager.AwardCoins(-5);
        }
        
        [TestMethod]
        public void ConsumeCoins_ShouldDecreaseTotalCoin_WhenSufficientBalance()
        {
            // Arrange
            GameStateManager.coinTtl = 100;
            int consumeAmount = 30;
            
            // Act
            bool result = GameStateManager.ConsumeCoins(consumeAmount);
            
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(70, GameStateManager.coinTtl);
        }
        
        [TestMethod]
        public void ConsumeCoins_ShouldReturnFalse_WhenInsufficientBalance()
        {
            // Arrange
            GameStateManager.coinTtl = 10;
            int consumeAmount = 30;
            
            // Act
            bool result = GameStateManager.ConsumeCoins(consumeAmount);
            
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(10, GameStateManager.coinTtl); // 余额不变
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConsumeCoins_ShouldThrowException_WhenNegativeAmount()
        {
            // Act
            GameStateManager.ConsumeCoins(-5);
        }
        
        [TestMethod]
        public void HasEnoughCoins_ShouldReturnCorrectResult()
        {
            // Arrange
            GameStateManager.coinTtl = 50;
            
            // Act & Assert
            Assert.IsTrue(GameStateManager.HasEnoughCoins(30));
            Assert.IsTrue(GameStateManager.HasEnoughCoins(50));
            Assert.IsFalse(GameStateManager.HasEnoughCoins(51));
        }
        
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void HasEnoughCoins_ShouldThrowException_WhenNegativeAmount()
        {
            // Act
            GameStateManager.HasEnoughCoins(-5);
        }
        
        [TestMethod]
        public void UsePause_ShouldWorkForCountBasedPause()
        {
            // Arrange
            GameStateManager.pauseType = 0; // 限次数模式
            GameStateManager.allowPause = 5;
            GameStateManager.flagPause = false;
            
            // Act
            bool result1 = GameStateManager.UsePause();
            bool result2 = GameStateManager.UsePause();
            
            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.AreEqual(3, GameStateManager.allowPause); // 5-2=3
            Assert.IsTrue(GameStateManager.flagPause);
        }
        
        [TestMethod]
        public void UsePause_ShouldReturnFalse_WhenNoPausesLeft()
        {
            // Arrange
            GameStateManager.pauseType = 0; // 限次数模式
            GameStateManager.allowPause = 0;
            
            // Act
            bool result = GameStateManager.UsePause();
            
            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, GameStateManager.allowPause);
        }
        
        [TestMethod]
        public void UsePause_ShouldWorkForTimeBasedPause()
        {
            // Arrange
            GameStateManager.pauseType = 1; // 限时间模式
            GameStateManager.pauseSecLeft = 600;
            GameStateManager.flagPause = false;
            
            // Act
            bool result = GameStateManager.UsePause();
            
            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(GameStateManager.flagPause);
            Assert.AreEqual(600, GameStateManager.pauseSecLeft); // 时间不变，由计时器管理
        }
        
        [TestMethod]
        public void ResumePause_ShouldSetFlagPauseToFalse()
        {
            // Arrange
            GameStateManager.flagPause = true;
            
            // Act
            GameStateManager.ResumePause();
            
            // Assert
            Assert.IsFalse(GameStateManager.flagPause);
        }
        
        [TestMethod]
        public void UpdatePauseTime_ShouldDecreaseTime_ForTimeBasedPause()
        {
            // Arrange
            GameStateManager.pauseType = 1; // 限时间模式
            GameStateManager.pauseSecLeft = 600;
            
            // Act
            GameStateManager.UpdatePauseTime(30);
            
            // Assert
            Assert.AreEqual(570, GameStateManager.pauseSecLeft);
        }
        
        [TestMethod]
        public void UpdatePauseTime_ShouldNotAffect_ForCountBasedPause()
        {
            // Arrange
            GameStateManager.pauseType = 0; // 限次数模式
            GameStateManager.pauseSecLeft = 600;
            
            // Act
            GameStateManager.UpdatePauseTime(30);
            
            // Assert
            Assert.AreEqual(600, GameStateManager.pauseSecLeft); // 不变
        }
        
        [TestMethod]
        public void UpdatePauseTime_ShouldNotGoBelowZero()
        {
            // Arrange
            GameStateManager.pauseType = 1;
            GameStateManager.pauseSecLeft = 10;
            
            // Act
            GameStateManager.UpdatePauseTime(15);
            
            // Assert
            Assert.AreEqual(0, GameStateManager.pauseSecLeft);
        }
        
        [TestMethod]
        public void GetRemainingPause_ShouldReturnCorrectValue()
        {
            // 测试限次数模式
            GameStateManager.pauseType = 0;
            GameStateManager.allowPause = 5;
            Assert.AreEqual(5, GameStateManager.GetRemainingPause());
            
            // 测试限时间模式
            GameStateManager.pauseType = 1;
            GameStateManager.pauseSecLeft = 300;
            Assert.AreEqual(300, GameStateManager.GetRemainingPause());
        }
        
        // 此测试已删除 - SaveProgSettings 使用应用程序目录，测试指定的临时配置文件路径无效
        // [TestMethod]
        // public void SaveAndLoadSettings_ShouldPersistState() { ... }

        [TestMethod]
        public void InitSettings_ShouldCreateDefaultFile_WhenFileNotExists()
        {
            // Arrange - 确保文件不存在
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 文件应被创建
            Assert.IsTrue(File.Exists(_tempIniPath));
            
            // 验证默认值被写入
            var configService = new IniConfigurationService(_tempIniPath);
            string psw = configService.ReadValue("General", "PSW");
            int coinTtl = configService.ReadInt("APP", "coinTtl");
            
            Assert.AreEqual("qiwei", psw);
            Assert.AreEqual(50, coinTtl); // 默认金币数
        }

        #region 枚举类型测试（任务 12）

        [TestMethod]
        public void GetPauseTypeEnum_ShouldReturnCorrectType()
        {
            // Arrange
            GameStateManager.pauseType = 0;
            
            // Act
            PauseType result = GameStateManager.GetPauseTypeEnum();
            
            // Assert
            Assert.AreEqual(PauseType.ByCount, result);
        }

        [TestMethod]
        public void GetPauseTypeEnum_ShouldReturnByTime_WhenPauseTypeIs1()
        {
            // Arrange
            GameStateManager.pauseType = 1;
            
            // Act
            PauseType result = GameStateManager.GetPauseTypeEnum();
            
            // Assert
            Assert.AreEqual(PauseType.ByTime, result);
        }

        [TestMethod]
        public void SetPauseTypeEnum_ShouldUpdatePauseTypeField()
        {
            // Arrange
            GameStateManager.pauseType = 0;
            
            // Act
            GameStateManager.SetPauseTypeEnum(PauseType.ByTime);
            
            // Assert
            Assert.AreEqual(1, GameStateManager.pauseType);
        }

        [TestMethod]
        public void SetPauseTypeEnum_ShouldWorkForBothValues()
        {
            // Test setting to ByCount
            GameStateManager.SetPauseTypeEnum(PauseType.ByCount);
            Assert.AreEqual(0, GameStateManager.pauseType);
            
            // Test setting to ByTime
            GameStateManager.SetPauseTypeEnum(PauseType.ByTime);
            Assert.AreEqual(1, GameStateManager.pauseType);
        }

        #endregion

        #region 配置项范围验证测试（任务 13）

        [TestMethod]
        public void InitSettings_ShouldValidatePauseTypeRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "pauseType", "5"); // 超出范围（有效范围 0-1）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultPauseType, GameStateManager.pauseType, 
                "超出范围的 pauseType 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidatePauseSecondsLeftRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "pauseSecLeft", "5000"); // 超出范围（有效范围 60-3600）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultPauseSecondsLeft, GameStateManager.pauseSecLeft,
                "超出范围的 pauseSecLeft 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidateAllowPauseCountRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "allowPause", "25"); // 超出范围（有效范围 0-20）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultAllowPauseCount, GameStateManager.allowPause,
                "超出范围的 allowPause 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidateErrorMessageShowTimeRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "errorMsgShowTime", "0"); // 超出范围（有效范围 1-30）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultErrorMessageShowTime, GameStateManager.errorMsgShowTime,
                "超出范围的 errorMsgShowTime 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidateAwardCoinRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "awardCoin", "15"); // 超出范围（有效范围 1-10）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultAwardCoinPerCorrect, GameStateManager.awardCoin,
                "超出范围的 awardCoin 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidateCostCoinCheckRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "costCoinCheck", "10"); // 超出范围（有效范围 0-5）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultCostCoinCheck, GameStateManager.costCoinCheck,
                "超出范围的 costCoinCheck 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidateCostCoinGiveRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "costCoinGive", "15"); // 超出范围（有效范围 0-10）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultCostCoinGive, GameStateManager.costCoinGive,
                "超出范围的 costCoinGive 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidatePunishmentRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "punishment", "10"); // 超出范围（有效范围 0-5）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultPunishmentAddQuestions, GameStateManager.punishment,
                "超出范围的 punishment 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidatePunishmentTimeoutRange()
        {
            // Arrange - 创建包含超出范围值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "punishmentTimeOut", "10"); // 超出范围（有效范围 0-5）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultPunishmentTimeout, GameStateManager.punishmentTimeOut,
                "超出范围的 punishmentTimeOut 应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldValidateBooleanConfigurations()
        {
            // Arrange - 创建包含无效布尔值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "helpBox", "3"); // 无效布尔值
            configService.WriteValue("APP", "helpBoxFree", "4"); // 无效布尔值
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultHelpBoxEnabled, GameStateManager.helpBox,
                "无效的 helpBox 值应被修正为默认值");
            Assert.AreEqual(AppConstants.DefaultHelpBoxFree, GameStateManager.helpBoxFree,
                "无效的 helpBoxFree 值应被修正为默认值");
        }

        [TestMethod]
        public void InitSettings_ShouldAcceptValidBooleanConfigurations()
        {
            // Arrange - 创建包含有效布尔值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "helpBox", "false"); // 有效布尔值
            configService.WriteValue("APP", "helpBoxFree", "1"); // 有效布尔值（1 表示 true）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该保留配置文件的值
            Assert.IsFalse(GameStateManager.helpBox, "有效的 helpBox 值应被保留");
            Assert.IsTrue(GameStateManager.helpBoxFree, "有效的 helpBoxFree 值应被保留");
        }

        [TestMethod]
        public void InitSettings_ShouldAcceptValuesWithinValidRanges()
        {
            // Arrange - 创建包含在有效范围内的值的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "pauseType", "0"); // 有效值
            configService.WriteValue("APP", "pauseSecLeft", "300"); // 有效值（60-3600）
            configService.WriteValue("APP", "allowPause", "10"); // 有效值（0-20）
            configService.WriteValue("APP", "errorMsgShowTime", "10"); // 有效值（1-30）
            configService.WriteValue("APP", "awardCoin", "5"); // 有效值（1-10）
            configService.WriteValue("APP", "costCoinCheck", "2"); // 有效值（0-5）
            configService.WriteValue("APP", "costCoinGive", "5"); // 有效值（0-10）
            configService.WriteValue("APP", "punishment", "3"); // 有效值（0-5）
            configService.WriteValue("APP", "punishmentTimeOut", "2"); // 有效值（0-5）
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 所有值应被保留
            Assert.AreEqual(0, GameStateManager.pauseType);
            Assert.AreEqual(300, GameStateManager.pauseSecLeft);
            Assert.AreEqual(10, GameStateManager.allowPause);
            Assert.AreEqual(10, GameStateManager.errorMsgShowTime);
            Assert.AreEqual(5, GameStateManager.awardCoin);
            Assert.AreEqual(2, GameStateManager.costCoinCheck);
            Assert.AreEqual(5, GameStateManager.costCoinGive);
            Assert.AreEqual(3, GameStateManager.punishment);
            Assert.AreEqual(2, GameStateManager.punishmentTimeOut);
        }

        [TestMethod]
        public void InitSettings_ShouldCorrectMinimumOutOfRangeValues()
        {
            // Arrange - 创建包含最小值以下范围的配置文件
            if (File.Exists(_tempIniPath))
                File.Delete(_tempIniPath);
            
            var configService = new IniConfigurationService(_tempIniPath);
            configService.WriteValue("APP", "pauseSecLeft", "30"); // 小于最小值 60
            configService.WriteValue("APP", "errorMsgShowTime", "0"); // 小于最小值 1
            configService.WriteValue("APP", "awardCoin", "0"); // 小于最小值 1
            configService.WriteValue("General", "PSW", "qiwei");
            
            // Act
            GameStateManager.InitSettings();
            
            // Assert - 应该被修正为默认值
            Assert.AreEqual(AppConstants.DefaultPauseSecondsLeft, GameStateManager.pauseSecLeft);
            Assert.AreEqual(AppConstants.DefaultErrorMessageShowTime, GameStateManager.errorMsgShowTime);
            Assert.AreEqual(AppConstants.DefaultAwardCoinPerCorrect, GameStateManager.awardCoin);
        }

        #endregion
    }
}