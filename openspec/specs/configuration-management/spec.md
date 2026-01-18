## Purpose
配置管理模块负责应用程序配置的持久化，包括金币数量、密码等关键数据的存储与加载。通过 INI 配置文件实现配置管理，确保用户数据在程序关闭后能够恢复。
## Requirements
### Requirement: 配置管理
(SHALL) 系统应使用 INI 文件存储配置，包括金币数、密码等设置，并在启动时加载配置。系统应在答题完成时立即保存用户金币数量到配置文件，确保程序异常关闭时数据不丢失。

#### Scenario: 配置文件不存在时创建
- **GIVEN** 应用程序首次运行，配置文件（bbmath.cfg）不存在
- **WHEN** 初始化配置时
- **THEN** 创建配置文件，写入默认值（金币=10，密码=qiwei）

#### Scenario: 读取配置成功
- **GIVEN** 配置文件存在，Section=APP, Key=coinTtl, Value=80
- **WHEN** 应用程序启动
- **THEN** 金币数初始化为 80

#### Scenario: 读取配置失败（Key不存在）
- **GIVEN** 配置文件中不存在指定的 Section 或 Key
- **WHEN** 尝试读取配置
- **THEN** 返回空字符串或默认值，并记录调试信息

#### Scenario: 写入配置
- **GIVEN** 配置文件已存在
- **WHEN** 修改金币数为 100 并保存
- **THEN** 配置文件更新，下次启动时加载新值

#### Scenario: 保存配置失败处理
- **GIVEN** 写入配置文件时发生异常（如权限不足）
- **WHEN** 捕获到异常
- **THEN** 记录错误信息，继续运行但不保存配置

#### Scenario: 保存暂停配置时使用配置值而非运行时值
- **GIVEN** 限时间暂停，配置时间为600秒（pauseSecondsLeftConfig），练习中剩余时间已减少至450秒（pauseSecLeft）
- **WHEN** 练习完成时调用 SaveProgSettings()
- **THEN** 配置文件中保存 pauseSecondsLeftConfig 的值（600秒），不保存运行时的剩余时间（pauseSecLeft）

#### Scenario: 配置包含密码
- **GIVEN** 配置文件中 Section=General, Key=PSW, Value="qiwei"
- **WHEN** 用户进入设置界面
- **THEN** 需要输入正确密码才能修改配置

#### Scenario: 练习完成后立即保存金币
- **GIVEN** 用户完成练习答题
- **WHEN** 系统保存练习记录到日志文件
- **THEN** 系统应同时调用 SaveProgSettings() 保存金币到 bbmath.cfg 文件，配置文件中的 coinTtl 值应与当前金币数量一致

#### Scenario: 程序正常退出时保存金币
- **GIVEN** 程序正常关闭
- **WHEN** 触发 FormClosing 事件
- **THEN** 系统应调用 SaveProgSettings() 保存金币到配置文件，确保所有配置项都已持久化

#### Scenario: 答题完成后程序异常关闭
- **GIVEN** 用户完成练习答题
- **WHEN** 系统立即保存金币到配置文件后程序异常关闭
- **THEN** 重新打开程序后金币数量应保持不变，配置文件中的 coinTtl 值应为答题完成时的值

### Requirement: 金币保存日志反馈
(SHALL) 系统应在保存金币到配置文件时输出日志信息，用于调试和跟踪。

#### Scenario: 保存金币时记录日志
- **GIVEN** 系统需要保存金币数量到配置文件
- **WHEN** 系统调用 SaveProgSettings() 保存金币
- **THEN** 日志应记录保存的金币数量，消息格式为："金币已保存到配置文件: {金币数量}"

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

#### Scenario: 配置值缺失时使用默认值
- **GIVEN** 配置文件中不存在 Key=coinTtl
- **WHEN** 调用 `IConfigurationService.ReadInt("APP", "coinTtl")` 并指定默认值为 10
- **THEN** 返回默认值 10

### Requirement: 配置值与运行时值分离
(SHALL) 系统应区分配置值（Configuration Value）和运行时值（Runtime Value），确保配置文件中保存固定的配置值，而非运行时动态变化的值。

**配置值（Configuration Value）**：
- 定义：从配置文件读取的固定值，不会在运行时修改
- 目的：为每次练习提供一致的初始状态
- 示例：`pauseSecondsLeftConfig`、`allowPauseConfig`

**运行时值（Runtime Value）**：
- 定义：在练习过程中动态变化的值
- 目的：跟踪当前会话的实时状态
- 示例：`pauseSecLeft`、`allowPause`

**生命周期**：
1. 初始化：配置值 = 运行时值（从配置文件加载）
2. 练习开始：运行时值重置为配置值
3. 练习进行中：运行时值不断减少
4. 练习结束：保存配置值（不保存运行时值）
5. 下次练习：重复上述循环

#### Scenario: 配置值与运行时值初始化
- **GIVEN** 配置文件中暂停时间为600秒
- **WHEN** 系统初始化
- **THEN** `pauseSecondsLeftConfig` = 600，`pauseSecLeft` = 600

#### Scenario: 练习开始时运行时值重置
- **GIVEN** 配置时间为600秒，上次练习后剩余时间为450秒
- **WHEN** 用户点击"开始"按钮
- **THEN** `pauseSecLeft` 重置为600秒，`pauseSecondsLeftConfig` 保持600秒不变

#### Scenario: 练习进行中运行时值变化
- **GIVEN** 暂停时间为600秒，用户使用了60秒暂停
- **WHEN** 暂停窗口关闭
- **THEN** `pauseSecLeft` 减少为540秒，`pauseSecondsLeftConfig` 保持600秒不变

#### Scenario: 练习结束时保存配置值
- **GIVEN** 配置时间为600秒，运行时剩余时间为200秒
- **WHEN** 调用 SaveProgSettings() 保存配置
- **THEN** 配置文件保存 pauseSecondsLeftConfig 的值（600秒），不保存 pauseSecLeft 的值（200秒）

#### Scenario: 点击"再练一次"时运行时值立即重置
- **GIVEN** 配置时间为600秒，运行时剩余时间为200秒
- **WHEN** 用户点击"再练一次"按钮
- **THEN** `pauseSecLeft` 立即重置为600秒，lbAllowPause 立即显示"600 秒"

#### Scenario: 读取损坏的整数值
- **GIVEN** 配置文件中 Key=coinTtl 的值为非数字字符串 "abc"
- **WHEN** 调用 `IConfigurationService.ReadInt("APP", "coinTtl")`
- **THEN** 返回默认值（10）并记录警告日志

