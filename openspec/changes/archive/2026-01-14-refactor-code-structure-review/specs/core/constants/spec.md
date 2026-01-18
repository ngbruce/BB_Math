## ADDED Requirements

### Requirement: 应用常量管理
系统必须提供一个集中的 `AppConstants` 类来管理所有应用程序级别的常量，包括默认值、文件名、超时时间和其他配置相关的字面量。

#### Scenario: 集中访问常量
- **WHEN** 应用程序的任何组件需要访问默认值、文件名或超时时长
- **THEN** 组件必须使用 `AppConstants` 类而不是硬编码的字面量

#### Scenario: 常量值的一致性
- **GIVEN** 在 `AppConstants` 中定义了一个常量
- **WHEN** 多个组件引用这个常量
- **THEN** 所有组件必须从单一事实源访问相同的值

#### Scenario: 常量命名约定
- **GIVEN** 正在定义 `AppConstants` 类
- **WHEN** 添加新常量
- **THEN** 常量名必须遵循 PascalCase 命名规则，并清楚地表明其用途（例如：`DefaultPassword`、`ConfigFileName`、`MaxPauseDuration`）

### Requirement: 默认密码常量
系统必须定义一个常量，用于身份验证和验证中使用的默认密码值。

#### Scenario: 默认密码集中管理
- **GIVEN** 应用程序具有用于家长控制或设置访问的默认密码
- **WHEN** 任何验证逻辑检查默认密码
- **THEN** 逻辑必须使用 `AppConstants.DefaultPassword` 常量

### Requirement: 配置文件名常量
系统必须为应用程序中使用的所有配置文件名定义常量。

#### Scenario: 配置文件名
- **GIVEN** 应用程序使用 INI 文件或其他配置文件
- **WHEN** 构建文件路径或文件名
- **THEN** 文件名必须使用 `AppConstants` 中的常量（例如：`ConfigFileName`、`LogFileName`）

### Requirement: 时长和超时常量
系统必须为应用程序中使用的所有时长和超时值定义常量。

#### Scenario: 暂停时长管理
- **GIVEN** 应用程序具有可配置的暂停时长
- **WHEN** 设置或验证暂停时间
- **THEN** 值必须引用 `AppConstants.MaxPauseDuration` 或 `AppConstants.DefaultPauseDuration`

#### Scenario: 超时值
- **GIVEN** 应用程序对操作使用超时
- **WHEN** 指定超时
- **THEN** 值必须引用 `AppConstants` 中的相应常量

### Requirement: 默认游戏状态值
系统必须为默认游戏状态值定义常量，例如初始金币、分数或其他起始值。

#### Scenario: 初始游戏状态
- **GIVEN** 新游戏会话开始
- **WHEN** 为金币、分数或其他游戏状态设置初始值
- **THEN** 这些值必须引用 `AppConstants` 中的常量

### Requirement: 常量文档
`AppConstants` 中的所有常量必须包含解释其用途和有效值范围的 XML 文档注释。

#### Scenario: 常量文档
- **GIVEN** 向 `AppConstants` 添加新常量
- **WHEN** 定义常量
- **THEN** 它必须具有描述其用途和用法的 XML 文档注释

### Requirement: 常量可测试性
`AppConstants` 类必须是可测试的，以确保所有定义的常量都有效且适当。

#### Scenario: 常量验证
- **GIVEN** 为应用程序编写了单元测试
- **WHEN** 测试常量值
- **THEN** 测试必须验证所有常量具有非空/有效值并符合预期范围
