# Project Context

## 文档地图

本项目采用 OpenSpec 规范驱动开发，文档结构如下：

### 核心文档
- **openspec/AGENTS.md** - OpenSpec 使用指南
  - 提案创建流程
  - 规范文件格式
  - 命令行工具使用
  - 适合：需要创建变更提案时查阅

- **openspec/project.md** - 项目总体规范（本文档）
  - 技术栈和开发环境
  - 命名规范和代码风格
  - 核心架构和约束
  - 适合：了解项目整体规范、新手入门

- **docs/CODING_STANDARDS.md** - 开发快速参考
  - 场景化查阅指引
  - 核心约束速览
  - 快速导航和搜索技巧
  - 适合：日常开发快速查阅

### 功能规范文档
- **openspec/specs/** - 各功能模块详细规范
  - `configuration-management/` - 配置管理
  - `scoring-system/` - 金币系统
  - `pause-mechanism/` - 暂停机制
  - `logging/` - 日志系统
  - 适合：开发具体功能时查阅

### API 文档
- **docs/API_REFERENCE.md** - API 接口参考
  - 所有类和接口定义
  - 使用示例和代码片段
  - 适合：编码实现时查阅

### 新手建议阅读顺序
1. **project.md** → 了解项目整体规范
2. **CODING_STANDARDS.md** → 掌握快速查阅方法
3. **AGENTS.md** → 学习 OpenSpec 流程（需要时）

---

## Purpose
BB数学 (BB_Math) 是一款面向儿童的数学练习软件，通过游戏化的方式（金币奖励、辅助功能、时间限制等）帮助儿童练习加减乘除运算，包括有余数除法。软件提供实时反馈、进度跟踪、成绩记录等功能，旨在提高儿童数学运算能力。

## Tech Stack
- **Language**: C# 7.0+
- **Framework**: .NET Framework 4.6.2
- **UI Platform**: Windows Forms (WinForms)
- **Build System**: MSBuild
- **Configuration**: INI 文件存储 (Ini_File.cs)
- **Logging**: 文本文件日志系统 (bbmath.dat)
- **IDE**: Visual Studio 2015+

## Project Conventions

### Code Style
- **命名规范**：
  - PascalCase 用于类名、方法名、公共成员（如 `ExamObject`, `InitSettings`）
  - camelCase 用于局部变量、私有字段（如 `num1`, `examTtl`）
  - 匈牙利命名法用于UI控件（如 `tbResult`, `btnStart`, `lbCoinTTL`）
  - 常量使用全大写（如 `fileName`, `iniFileName`）
- **注释规范**：使用 XML 文档注释（///）为类、方法、属性添加说明，中文注释
- **代码组织**：按功能分组，相关方法集中放置
- **字符串处理**：优先使用 StringBuilder 处理大量字符串拼接
- **文件路径**：使用 `AppDomain.CurrentDomain.BaseDirectory` 获取应用根目录

### Architecture Patterns
- **静态配置类**：使用静态类 `bbMath` 集中管理全局配置和状态
- **数据对象模式**：使用 `ExamObject` 类封装题型配置和统计数据
- **枚举类型**：使用 `ExamType` 枚举定义数学运算类型
- **INI配置文件**：使用自定义 `Ini_File` 类读写配置
- **事件驱动**：基于 Windows Forms 事件模型处理用户交互

### Testing Strategy
- **当前状态**：项目暂无自动化测试
- **手动测试**：通过实际运行验证功能
- **调试输出**：使用 `Debug.Print()` 进行运行时调试
- **建议改进**：需要引入单元测试框架（如 NUnit 或 MSTest）

### Git Workflow
- **当前状态**：项目使用简单的主干开发
- **提交规范**：建议采用约定式提交（Conventional Commits）
- **分支策略**：建议采用 feature branch 工作流

## Spec File Format

### Spec 文件结构规范
OpenSpec 规范文件必须包含以下部分：

#### 主 Spec 文件（`openspec/specs/<capability>/spec.md`）
必须包含：
- `## Purpose` - 描述该能力的目的和范围，**至少50个字符**
- `## Requirements` - 包含所有需求和场景

**Purpose 编写规范：**
- 长度要求：至少50个字符（约15-20个汉字）
- 内容应包括：目的、范围、价值/目标
- 避免过于简略的描述

示例：
```markdown
## Purpose
日志系统记录每次练习的详细数据，包括开始时间、答题结果、用时、金币数等信息，便于后续分析和成绩追踪。

## Requirements

### Requirement: 需求名称
系统 SHALL [行为描述]

#### Scenario: 场景名称
- **GIVEN** [前置条件]
- **WHEN** [触发条件]
- **THEN** [预期结果]
```

**Purpose 编写对比：**
- ✅ 正确：日志系统记录每次练习的详细数据，包括开始时间、答题结果、用时、金币数等信息，便于后续分析和成绩追踪。
- ❌ 错误：日志系统记录练习数据（仅9个字符，不满足≥50字符要求）

#### Change Delta 文件（`openspec/changes/<id>/specs/<capability>/spec.md`）
使用操作前缀标记变更类型：
- `## ADDED Requirements` - 新增能力
- `## MODIFIED Requirements` - 修改现有行为
- `## REMOVED Requirements` - 移除废弃功能
- `## RENAMED Requirements` - 仅修改名称

### Requirement 编写规范

#### MUST/SHALL 关键字使用
1. **Requirement 文本必须包含 SHALL 或 MUST**
   - 正确：`(SHALL) 系统应使用 INI 文件存储配置`
   - 错误：`系统应使用 INI 文件存储配置`

2. **中文环境下的推荐格式**
   - 格式 A（推荐）：`(SHALL) 系统应使用 INI 文件存储配置`
     - 优点：符合自然语言，易于阅读
     - 适合在 requirement 开头添加 SHALL 前缀
   - 格式 B：`系统 SHALL 使用 INI 文件存储配置`
     - 优点：简洁明确
     - 适合中文+英文混合的场景
   - 格式 C：`系统 (SHALL) 使用 INI 文件存储配置`
     - 注意：这种格式 openspec 可能无法正确识别 SHALL

3. **禁止使用**
   - `应`、`应该`、`需要` 等弱表述
   - `will`、`can`、`may`（除非明确表示非规范要求）

### Scenario 编写规范

#### 标题格式
- 必须使用四级标题：`#### Scenario: 场景名称`
- **正确示例**：
  ```markdown
  #### Scenario: 用户登录成功
  - **WHEN** 输入有效凭证
  - **THEN** 返回 JWT token
  ```
- **错误示例**（不要使用）：
  ```markdown
  - **Scenario: 用户登录          ❌
  **Scenario**: 用户登录        ❌
  ### Scenario: 用户登录       ❌
  ```

#### 场景元素
每个场景必须包含：
- `**GIVEN** - 前置条件
- `**WHEN** - 触发条件
- `**THEN** - 预期结果

### MODIFIED 操作规范

#### 完整性要求
当使用 `## MODIFIED Requirements` 时：
1. **必须复制完整的 requirement 内容**
   - 从 `### Requirement: ...` 到所有 scenarios
   - 不要只复制修改部分，否则会导致原有内容丢失

2. **必须保持 requirement 标题一致**
   - 标题必须与主 spec 文件完全一致（包括空格）
   - openspec 使用 `trim(header)` 匹配，空格会被忽略

3. **更新内容而非替换**
   - 复制原文后修改需要变更的部分
   - 不要从头重写，否则会丢失原有的 scenarios

#### 正确示例
```markdown
## MODIFIED Requirements

### Requirement: 配置管理
系统 SHALL 使用 INI 文件存储配置，包括金币数、密码等设置，并在启动时加载配置。系统 SHALL 在答题完成时立即保存用户金币数量到配置文件。

#### Scenario: 配置文件不存在时创建
- **GIVEN** 应用程序首次运行，配置文件（bbmath.cfg）不存在
- **WHEN** 初始化配置时
- **THEN** 创建配置文件，写入默认值

#### Scenario: 练习完成后立即保存金币
- **GIVEN** 用户完成练习答题
- **WHEN** 系统保存练习记录到日志文件
- **THEN** 系统应同时调用 SaveProgSettings() 保存金币到 bbmath.cfg 文件
```

### 归档常见错误

#### 1. Requirement 标题不匹配
**错误信息**：`MODIFIED "配置管理" must contain SHALL or MUST`
**原因**：delta 文件中的 requirement 标题与主 spec 文件不匹配
**解决**：确保标题完全一致（包括空格和标点）

#### 2. 缺少 Purpose 部分
**错误信息**：`Spec must have a Purpose section. Missing required sections`
**原因**：主 spec 文件缺少 `## Purpose` 部分
**解决**：添加 Purpose 部分，简要描述该能力的目的

#### 3. Scenario 格式错误
**错误信息**：`Requirement must have at least one scenario`
**原因**：scenario 标题格式不正确（使用了三级或项目符号）
**解决**：使用 `#### Scenario: 名称` 格式

#### 4. SHALL/MUST 关键字未识别
**错误信息**：`Requirement must contain SHALL or MUST`
**原因**：requirement 文本中缺少 SHALL 或 MUST 关键字
**解决**：在 requirement 文本中添加 `SHALL` 或 `MUST`

#### 5. Purpose 部分字数不足
**错误信息**：`Purpose section is too brief (less than 50 characters)`
**原因**：Purpose 部分字数少于50个字符
**解决**：扩展Purpose内容，至少50个字符（约15-20个汉字），应包括目的、范围、价值/目标

#### 6. 验证失败但无详细信息
**错误信息**：openspec validate 只显示 ✗，没有详细错误信息
**原因**：默认输出模式不显示详细错误
**解决**：使用 `--json` 参数查看详细错误信息
```bash
# 查看单个规范的详细错误
openspec validate spec-name --type spec --strict --json

# 查看所有规范的详细错误
openspec validate --specs --strict --json
```

**重要说明**：在 `--strict` 模式下，WARNING 级别的错误会被视为失败，因此所有WARNING都需要修复。

### 快速检查清单

在归档或提交 change 前，检查以下项目：
- [ ] 所有 MODIFIED 的 requirement 是否包含 SHALL/MUST
- [ ] requirement 标题是否与主 spec 文件完全一致
- [ ] 是否复制了完整的 requirement 块（包括所有 scenarios）
- [ ] scenario 标题是否使用 `#### Scenario:` 格式
- [ ] 主 spec 文件是否包含 `## Purpose` 和 `## Requirements` 部分
- [ ] Purpose 部分是否至少50个字符
- [ ] delta 文件的操作前缀是否正确（ADDED/MODIFIED/REMOVED/RENAMED）

### 验证最佳实践

开发过程中建议遵循以下验证流程：

#### 1. 编写规范后立即验证
```bash
# 验证单个规范文件
openspec validate spec-name --type spec --strict --json

# 或使用相对路径
openspec validate specs/help-system --strict --json
```

#### 2. 定期运行完整验证
```bash
# 验证所有规范文件
openspec validate --specs --strict --json

# 验证所有变更提案
openspec validate --changes --strict --json
```

#### 3. 遇到失败时使用 --json 查看详情
```bash
# 基本验证（只显示 ✓/✗）
openspec validate spec-name --strict

# 详细验证（显示具体错误）
openspec validate spec-name --strict --json
```

#### 4. 特别注意 WARNING 级别
- 在 `--strict` 模式下，WARNING 会被视为错误导致验证失败
- 常见的 WARNING：
  - `Purpose section is too brief (less than 50 characters)`
  - 其他格式或内容警告
- 所有 WARNING 都必须修复才能通过严格模式验证

#### 5. 验证时机建议
- ✅ 编写完新的 spec 文件后立即验证
- ✅ 修改现有 spec 后立即验证
- ✅ 创建 change proposal 时验证
- ✅ 归档 change 前验证
- ✅ 提交代码前运行完整验证

## Domain Context

### 核心概念
- **ExamType (题型枚举)**：定义了五种数学运算类型
  - Addition (加法)
  - Subtraction (减法)
  - Multiplication (乘法)
  - DivisionNoRemainder (无余数除法)
  - DivisionWithRemainder (有余数除法)

- **ExamObject (题型对象)**：封装特定题型的所有配置
  - 百分比、时间限制、整数/小数位数
  - 正确/错误数量统计、已用时间
  - 是否允许负数结果、小数支持

- **bbMath (静态配置类)**：全局状态管理
  - 金币系统（奖励、消耗、检查花费）
  - 暂停机制（次数限制、时间限制）
  - 进程黑名单（防止使用计算器、浏览器等）
  - 惩罚机制（错误加题、超时加题）

- **金币经济系统**：
  - 答对奖励金币（`awardCoin`）
  - 检查辅助花费金币（`costCoinCheck`）
  - 给出答案花费金币（`costCoinGive`）
  - 全对通关额外奖励

### 关键业务流程
1. **启动流程**：加载配置 → 初始化题型 → 显示主界面
2. **答题流程**：生成题目 → 用户输入 → 验证答案 → 更新统计 → 金币结算
3. **暂停流程**：停止计时 → 显示暂停窗口 → 恢复计时
4. **结束流程**：计算成绩 → 保存日志 → 奖励结算

### 文件格式
- **配置文件** (`bbmath.cfg`)：INI格式，存储金币、密码等设置
- **日志文件** (`bbmath.dat`)：文本格式，记录每次练习的详细数据

## Development Environment Constraints
- **VS Code 职责**：本环境仅用于代码编写和文档维护，**不能编译和调试 C# 工程**
- **VS 2019 职责**：所有编译、调试、运行必须在 Visual Studio 2019 IDE 中完成
- **AI 助手职责**：
  - 当需要编译或调试时，必须提醒用户在 VS 2019 中进行
  - 当新增代码文件或文件夹，必须提醒用户在 VS 2019 中添加到项目，否则编译时不会包含这些文件
  - 要求用户反馈编译错误、运行时异常等信息
  - 根据用户反馈的问题进行代码修正
  - **禁止操作**：AI 助手**不得**修改或新建任何窗体设计文件（`*.Designer.cs`）
  - **窗体/控件修改**：当需要添加、修改窗体或控件，或添加控件事件处理函数时，AI 助手必须：
    1. 暂停编程工作
    2. 提供详细的修改说明
    3. 要求用户在 VS IDE 的界面设计器中手动操作
    4. 等待用户确认完成后再继续

## Important Constraints
- **.NET Framework 限制**：必须使用 .NET Framework 4.6.2（兼容旧系统）
- **Windows 平台**：仅支持 Windows 操作系统
- **单用户应用**：当前设计为单用户本地应用，无网络功能
- **文件权限**：需要读写应用目录下的配置文件和日志文件
- **进程管理**：需要管理员权限才能结束黑名单中的进程
- **设计文件保护**：所有 `*.Designer.cs` 文件只能由 VS IDE 的窗体设计器生成和修改

## External Dependencies
- **系统组件**：依赖 Windows Forms 和 .NET Framework 运行时
- **进程监控**：通过 Process 类监控系统进程（计算器、浏览器等）
- **文件系统**：依赖本地文件系统进行配置和日志存储
- **无第三方库**：项目未使用任何 NuGet 包或第三方库
- **IDE 依赖**：依赖 Visual Studio 2019 进行窗体设计和编译调试
