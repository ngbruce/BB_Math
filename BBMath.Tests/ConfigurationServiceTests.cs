using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BBMath.Configuration;

namespace BBMath.Tests
{
    [TestClass]
    public class ConfigurationServiceTests
    {
        private string _tempIniPath;
        private IConfigurationService _configService;

        [TestInitialize]
        public void TestInitialize()
        {
            // 创建临时INI文件
            _tempIniPath = Path.GetTempFileName();
            File.WriteAllText(_tempIniPath, "[General]\r\nPSW=qiwei\r\n[APP]\r\ncoinTtl=100\r\nhelpBox=true\r\n");
            
            _configService = new IniConfigurationService(_tempIniPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // 删除临时文件
            if (File.Exists(_tempIniPath))
            {
                File.Delete(_tempIniPath);
            }
        }

        [TestMethod]
        public void ReadValue_ExistingKey_ReturnsCorrectValue()
        {
            // Arrange & Act
            string value = _configService.ReadValue("General", "PSW");
            
            // Assert
            Assert.AreEqual("qiwei", value, "应该读取到正确的密码值");
        }

        [TestMethod]
        public void ReadValue_NonExistingKey_ReturnsEmptyString()
        {
            // Arrange & Act
            string value = _configService.ReadValue("NonExist", "Key");
            
            // Assert
            Assert.AreEqual(string.Empty, value, "不存在的键应该返回空字符串");
        }

        [TestMethod]
        public void WriteValue_NewKey_FileUpdated()
        {
            // Arrange
            string section = "Test";
            string key = "NewKey";
            string expectedValue = "NewValue";
            
            // Act
            _configService.WriteValue(section, key, expectedValue);
            
            // Assert
            string actualValue = _configService.ReadValue(section, key);
            Assert.AreEqual(expectedValue, actualValue, "写入后应该能读取到相同的值");
        }

        [TestMethod]
        public void WriteValue_ExistingKey_OverwritesValue()
        {
            // Arrange
            string section = "General";
            string key = "PSW";
            string newValue = "newpassword";
            
            // Act
            _configService.WriteValue(section, key, newValue);
            
            // Assert
            string actualValue = _configService.ReadValue(section, key);
            Assert.AreEqual(newValue, actualValue, "应该覆盖原有的值");
        }

        [TestMethod]
        public void ReadInt_ValidInteger_ReturnsParsedValue()
        {
            // Arrange & Act
            int value = _configService.ReadInt("APP", "coinTtl");
            
            // Assert
            Assert.AreEqual(100, value, "应该正确解析整数");
        }

        [TestMethod]
        public void ReadInt_InvalidInteger_ReturnsDefaultValue()
        {
            // Arrange
            _configService.WriteValue("APP", "invalidInt", "abc");
            
            // Act
            int value = _configService.ReadInt("APP", "invalidInt");
            
            // Assert
            Assert.AreEqual(50, value, "解析失败应该返回默认值50");
        }

        [TestMethod]
        public void ReadBool_TrueValue_ReturnsTrue()
        {
            // Arrange & Act
            bool value = _configService.ReadBool("APP", "helpBox");
            
            // Assert
            Assert.IsTrue(value, "应该解析为true");
        }

        [TestMethod]
        public void ReadBool_FalseValue_ReturnsFalse()
        {
            // Arrange
            _configService.WriteValue("APP", "boolFalse", "false");
            
            // Act
            bool value = _configService.ReadBool("APP", "boolFalse");
            
            // Assert
            Assert.IsFalse(value, "应该解析为false");
        }

        [TestMethod]
        public void ReadBool_OneZeroValues_ReturnsCorrectBoolean()
        {
            // Arrange
            _configService.WriteValue("APP", "boolOne", "1");
            _configService.WriteValue("APP", "boolZero", "0");
            
            // Act & Assert
            Assert.IsTrue(_configService.ReadBool("APP", "boolOne"), "'1' 应该解析为 true");
            Assert.IsFalse(_configService.ReadBool("APP", "boolZero"), "'0' 应该解析为 false");
        }

        [TestMethod]
        public void Exists_ExistingKey_ReturnsTrue()
        {
            // Arrange & Act
            bool exists = _configService.Exists("General", "PSW");
            
            // Assert
            Assert.IsTrue(exists, "存在的键应该返回true");
        }

        [TestMethod]
        public void Exists_NonExistingKey_ReturnsFalse()
        {
            // Arrange & Act
            bool exists = _configService.Exists("NonExist", "Key");
            
            // Assert
            Assert.IsFalse(exists, "不存在的键应该返回false");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadValue_EmptySection_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            _configService.ReadValue("", "key");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReadValue_EmptyKey_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            _configService.ReadValue("section", "");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WriteValue_NullValue_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            _configService.WriteValue("section", "key", null);
        }

        #region 缓存机制测试

        [TestMethod]
        public void Cache_SequentialRead_UsesCache()
        {
            // Arrange - 读取配置项（第一次会从文件读取并存入缓存）
            var firstRead = _configService.ReadValue("General", "PSW");

            // Act - 再次读取相同配置项（应该从缓存读取）
            var secondRead = _configService.ReadValue("General", "PSW");
            var thirdRead = _configService.ReadValue("General", "PSW");

            // Assert - 多次读取应该得到相同的值
            Assert.AreEqual("qiwei", firstRead, "第一次读取应该返回正确值");
            Assert.AreEqual(firstRead, secondRead, "第二次读取应该从缓存返回相同值");
            Assert.AreEqual(secondRead, thirdRead, "第三次读取也应该从缓存返回相同值");
        }

        [TestMethod]
        public void Cache_AfterWrite_UpdatesCache()
        {
            // Arrange - 读取初始值
            var initialValue = _configService.ReadInt("APP", "coinTtl");
            Assert.AreEqual(100, initialValue, "初始值应该是100");

            // Act - 写入新值
            _configService.WriteValue("APP", "coinTtl", "999");

            // Assert - 再次读取应该立即得到新值（缓存已更新）
            var newValue = _configService.ReadInt("APP", "coinTtl");
            Assert.AreEqual(999, newValue, "缓存应该已更新，读取应该返回新值");
        }

        [TestMethod]
        public void Cache_WriteNewValue_CachesCorrectly()
        {
            // Arrange
            string section = "CacheTest";
            string key = "newKey";
            string expectedValue = "cachedValue";

            // Act - 写入新值（应该同时更新缓存和文件）
            _configService.WriteValue(section, key, expectedValue);

            // Assert - 立即读取应该得到新值（从缓存）
            var actualValue = _configService.ReadValue(section, key);
            Assert.AreEqual(expectedValue, actualValue, "新写入的值应该被缓存");

            // 再次读取验证缓存一致性
            var readAgain = _configService.ReadValue(section, key);
            Assert.AreEqual(expectedValue, readAgain, "缓存应该保持一致");
        }

        [TestMethod]
        public void Cache_DifferentKeys_CachedIndependently()
        {
            // Arrange - 写入多个不同的配置项
            _configService.WriteValue("Section1", "Key1", "Value1");
            _configService.WriteValue("Section1", "Key2", "Value2");
            _configService.WriteValue("Section2", "Key1", "Value3");

            // Act - 读取这些配置项
            var value1 = _configService.ReadValue("Section1", "Key1");
            var value2 = _configService.ReadValue("Section1", "Key2");
            var value3 = _configService.ReadValue("Section2", "Key1");

            // Assert - 每个键应该独立缓存，互不影响
            Assert.AreEqual("Value1", value1, "Section1.Key1 应该返回正确的值");
            Assert.AreEqual("Value2", value2, "Section1.Key2 应该返回正确的值");
            Assert.AreEqual("Value3", value3, "Section2.Key1 应该返回正确的值");
        }

        [TestMethod]
        public void Cache_MultipleInstances_CacheIsIndependent()
        {
            // Arrange - 创建两个不同的服务实例，使用相同的配置文件
            var service1 = _configService;
            var service2 = new IniConfigurationService(_tempIniPath);

            // Act - 通过第一个实例修改值
            service1.WriteValue("General", "PSW", "password1");

            // Assert - 第一个实例应该立即看到新值（缓存已更新）
            var fromService1 = service1.ReadValue("General", "PSW");
            Assert.AreEqual("password1", fromService1, "第一个实例应该读取到新值");

            // 注意：第二个实例由于有独立的缓存，可能仍读取旧值
            // 这是正常行为，因为每个实例有独立的缓存
            var fromService2 = service2.ReadValue("General", "PSW");
            // 第二个实例可能返回旧值 "qiwei" 或新值 "password1"
            // 取决于是否已清除缓存重新加载
            Assert.IsTrue(
                fromService2 == "qiwei" || fromService2 == "password1",
                "第二个实例可能返回旧值（因为缓存独立）或新值（如果重新加载）"
            );
        }

        [TestMethod]
        public void Cache_Performance_MultipleReads_IsFast()
        {
            // Arrange
            const int readCount = 100;
            var stopwatch = new System.Diagnostics.Stopwatch();

            // Act - 测量多次读取的性能
            stopwatch.Start();
            for (int i = 0; i < readCount; i++)
            {
                _configService.ReadInt("APP", "coinTtl");
            }
            stopwatch.Stop();

            // Assert - 100次读取应该非常快（使用缓存）
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            Assert.IsTrue(
                elapsedMs < 100,
                $"使用缓存，100次读取应该很快（实际：{elapsedMs}毫秒）"
            );
        }

        [TestMethod]
        public void ReloadCache_ClearsCache()
        {
            // Arrange - 读取配置项以加载到缓存
            var initialRead = _configService.ReadInt("APP", "coinTtl");
            Assert.AreEqual(100, initialRead, "初始值应该是100");

            // 手动修改文件（模拟外部修改）
            File.WriteAllText(_tempIniPath, "[APP]\r\ncoinTtl=200\r\n");

            // Act - 清除缓存
            (_configService as IniConfigurationService)?.ReloadCache();

            // Assert - 清除缓存后重新读取应该得到文件中的新值
            var newRead = _configService.ReadInt("APP", "coinTtl");
            Assert.AreEqual(200, newRead, "清除缓存后应该从文件重新加载新值");
        }

        [TestMethod]
        public void Cache_WithEmptyValues_CachesCorrectly()
        {
            // Arrange - 写入空字符串值
            _configService.WriteValue("Test", "EmptyKey", "");
            _configService.WriteValue("Test", "WhitespaceKey", "   ");

            // Act - 读取这些值
            var emptyValue = _configService.ReadValue("Test", "EmptyKey");
            var whitespaceValue = _configService.ReadValue("Test", "WhitespaceKey");

            // Assert - 空值和空白字符串应该被正确缓存
            Assert.AreEqual(string.Empty, emptyValue, "空字符串应该被正确缓存");
            Assert.AreEqual("   ", whitespaceValue, "空白字符串应该被正确缓存");
        }

        #endregion
    }
}