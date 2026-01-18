## ADDED Requirements

### Requirement: 配置管理
系统应使用 INI 文件存储配置，包括金币数、密码等设置，并在启动时加载配置。

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
