## MODIFIED Requirements
### Requirement: 配置管理
系统 SHALL 使用 INI 文件存储配置，包括金币数、密码等设置，并在启动时加载配置。系统 SHALL 在答题完成时立即保存用户金币数量到配置文件，确保程序异常关闭时数据不丢失。

#### Scenario: 配置文件不存在时创建
- **GIVEN** 应用程序首次运行，配置文件（bbmath.cfg）不存在
- **WHEN** 初始化配置时
- **THEN** 创建配置文件，写入默认值（金币=50，密码=qiwei）

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

#### Scenario: 配置包含密码
- **GIVEN** 配置文件中 Section=General, Key=PSW, Value="qiwei"
- **WHEN** 用户进入设置界面
- **THEN** 需要输入正确密码才能修改配置

#### Scenario: 练习完成后立即保存金币
- **GIVEN** 用户完成练习答题
- **WHEN** 系统保存练习记录到日志文件
- **THEN** 系统应同时调用 SaveProgSettings() 保存金币到 bbmath.cfg 文件
- **THEN** 配置文件中的 coinTtl 值应与当前金币数量一致

#### Scenario: 程序正常退出时保存金币
- **GIVEN** 程序正常关闭
- **WHEN** 触发 FormClosing 事件
- **THEN** 系统应调用 SaveProgSettings() 保存金币到配置文件
- **THEN** 确保所有配置项都已持久化

#### Scenario: 答题完成后程序异常关闭
- **GIVEN** 用户完成练习答题
- **WHEN** 系统立即保存金币到配置文件后程序异常关闭
- **THEN** 重新打开程序后金币数量应保持不变
- **THEN** 配置文件中的 coinTtl 值应为答题完成时的值

## ADDED Requirements
### Requirement: 金币保存日志反馈
系统 SHALL 在保存金币到配置文件时输出日志信息，用于调试和跟踪。

#### Scenario: 保存金币时记录日志
- **WHEN** 系统调用 SaveProgSettings() 保存金币
- **THEN** 日志应记录保存的金币数量
- **THEN** 日志消息格式为："金币已保存到配置文件: {金币数量}"
