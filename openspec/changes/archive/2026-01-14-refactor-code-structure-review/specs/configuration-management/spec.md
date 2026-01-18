## ADDED Requirements

### Requirement: 配置管理抽象接口
(SHALL) 系统应提供统一的配置管理接口（IConfigurationService），支持读取和写入应用程序配置，封装底层存储细节（如INI文件格式）。

#### Scenario: 读取字符串配置项
- **GIVEN** 配置文件中存在 Section=General, Key=PSW, Value="qiwei"
- **WHEN** 调用 `IConfigurationService.ReadValue("General", "PSW")`
- **THEN** 返回字符串 "qiwei"

#### Scenario: 读取不存在的配置项
- **GIVEN** 配置文件中不存在指定的 Section 或 Key
- **WHEN** 调用 `IConfigurationService.ReadValue("NonExist", "Key")`
- **THEN** 返回空字符串或默认值

#### Scenario: 写入配置项
- **GIVEN** 配置文件已存在
- **WHEN** 调用 `IConfigurationService.WriteValue("Section", "Key", "NewValue")`
- **THEN** 配置文件中新增或更新对应的配置项

#### Scenario: 读取整数配置项
- **GIVEN** 配置文件中存在 Section=APP, Key=coinTtl, Value="50"
- **WHEN** 调用 `IConfigurationService.ReadInt("APP", "coinTtl")`
- **THEN** 返回整数 50

#### Scenario: 读取布尔配置项
- **GIVEN** 配置文件中存在 Section=APP, Key=helpBox, Value="true"
- **WHEN** 调用 `IConfigurationService.ReadBool("APP", "helpBox")`
- **THEN** 返回布尔值 true

### Requirement: 强类型配置访问
(SHALL) 系统应提供强类型的配置类（AppSettings），将配置项封装为属性，避免魔法字符串。

#### Scenario: 访问金币总数配置
- **GIVEN** AppSettings 类已初始化
- **WHEN** 访问 `AppSettings.CoinTotal` 属性
- **THEN** 返回从配置文件读取的金币数值

#### Scenario: 修改并保存配置
- **GIVEN** AppSettings 实例
- **WHEN** 修改 `AppSettings.PauseType` 并调用 `Save()` 方法
- **THEN** 配置变更写入配置文件

### Requirement: 配置验证与默认值
(SHALL) 配置系统应验证配置值的合法性，并在配置缺失时提供合理的默认值。

#### Scenario: 读取损坏的整数值
- **GIVEN** 配置文件中 Key=coinTtl 的值为非数字字符串 "abc"
- **WHEN** 调用 `IConfigurationService.ReadInt("APP", "coinTtl")`
- **THEN** 返回默认值（50）并记录警告日志

#### Scenario: 配置文件不存在时创建
- **GIVEN** 应用程序首次运行，配置文件不存在
- **WHEN** 初始化配置系统
- **THEN** 自动创建配置文件并写入所有默认值
