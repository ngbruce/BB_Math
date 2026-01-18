# BB Math 开发快速参考

**版本**: v7.1.1
**最后更新**: 2026-01-18

> **重要提示**：本文档是 AI 编程助手的快速参考指南，包含最关键的开发规范和查阅指引。详细规范请查阅相关文档。

---

## 一、场景化查阅指引

### 根据对话场景快速定位文档

| 用户提问场景 | 应查阅文档 | 文档路径 | 关键词 |
|-------------|-----------|---------|--------|
| "帮我创建一个新功能的提案" | OpenSpec 使用指南 | `openspec/AGENTS.md` | proposal, change, spec |
| "金币系统是怎么设计的" | 功能模块规范 | `openspec/specs/[capability]/spec.md` | scoring-system, pause-mechanism |
| "IConfigurationService 怎么用" | API 接口参考 | `docs/API_REFERENCE.md` | 接口名、类名 |
| "命名规范是什么" | 项目总体规范 | `openspec/project.md` | 命名、代码风格 |
| "需要修改窗体上的按钮" | 窗体修改流程 | 本文档（第4节） | 控件、设计器 |
| "项目使用什么技术栈" | 项目总体规范 | `openspec/project.md` | Tech Stack |
| "需要添加/修改功能" | 功能模块规范 | `openspec/specs/` | 相关 capability |

### 新手建议阅读顺序
1. **openspec/project.md** - 了解项目整体规范和技术栈
2. **本文档** - 掌握快速查阅方法和核心约束
3. **openspec/AGENTS.md** - 学习 OpenSpec 提案流程（需要时）

---

## 二、核心约束速览（⚠️ 必须遵守）

### 2.1 环境职责分工

| 工具 | 职责范围 | 禁止操作 |
|------|---------|---------|
| **Visual Studio 2019** | 编译、调试、窗体设计、控件管理 | 无 |
| **VS Code** | 代码编写、文档维护、文件读写 | ❌ 编译/调试<br>❌ 修改 `*.Designer.cs` |
| **AI 助手** | 代码生成、文档整理 | ❌ 修改设计器文件<br>❌ 添加控件/事件 |

### 2.2 窗体/控件修改流程

**当需要修改窗体或控件时，AI 必须：**

1. **暂停编程**，提供详细操作说明
2. **要求用户在 VS 设计器中手动完成**
3. **等待用户确认**完成后再继续

**示例流程**：
```
AI: 需要在 Form1 上添加 Label 控件：
   1. 在 VS 设计器中打开 Form1.cs
   2. 拖拽 Label 到窗体
   3. 设置 Name = "lbNewLabel"
   4. 设置 Text = "新标签"
   5. 完成后请告诉我

用户: 已完成

AI: 好的，现在继续编写引用该控件的代码...
```

### 2.3 编译与调试流程

**AI 生成/修改代码后必须提醒用户：**
```
⚠️ 请回到 VS 2019 中编译项目
⚠️ 请报告是否有编译错误
```

**需要用户反馈的信息：**
- 编译是否成功（是/否）
- 错误消息和错误行号
- 运行时异常类型和堆栈信息
- 程序行为是否符合预期

### 2.4 绝对禁止事项

❌ **违反会导致严重问题：**
1. 修改 `*.Designer.cs` 文件（窗体设计器生成）
2. 在代码中使用魔法数字（必须在 `AppConstants.cs` 中定义常量）
3. 在 VS Code 中编译或调试 C# 项目
4. 直接实例化服务类（必须使用依赖注入）
5. 在窗体设计器中添加/修改控件或事件处理函数

---

## 三、快速命名规范

### 3.1 基本命名规则

| 类型 | 格式 | 示例 |
|------|------|------|
| 类名 | PascalCase | `GameStateManager` |
| 接口名 | PascalCase + I 前缀 | `IConfigurationService` |
| 方法名 | PascalCase | `GenerateProblem()` |
| 私有字段 | `_camelCase` | `private int _coinTotal;` |
| 常量 | PascalCase | `ConfigFileName` |
| UI 控件 | 匈牙利命名法 | `tbResult`, `btnStart` |

### 3.2 常量管理

**所有硬编码常量必须在 `Core/AppConstants.cs` 中定义**

```csharp
// ✅ 正确：使用集中管理的常量
int examTotal = AppConstants.DefaultExamTotal;
string configFile = AppConstants.ConfigFileName;

// ❌ 错误：魔法数字和硬编码字符串
int examTotal = 15;  // 魔法数字！
string configFile = "bbmath.cfg";  // 硬编码！
```

**详细规范请查阅：** `openspec/project.md`（第3节：代码风格）

---

## 四、OpenSpec 快速入门

### 4.1 何时需要创建提案

**需要创建提案：**
- 添加新功能或功能变更
- 进行破坏性更改（API、架构）
- 性能优化或安全模式更新

**不需要提案：**
- Bug 修复（恢复预期行为）
- 拼写错误、格式调整、注释
- 依赖更新（非破坏性）

### 4.2 创建提案三步曲

**第一步：查看现有规范**
```bash
# 查看所有功能模块
openspec list --specs

# 查看进行中的变更
openspec list
```

**第二步：创建提案文件**
```bash
mkdir -p openspec/changes/add-new-feature/specs/[capability]
```

创建三个文件：
- `proposal.md` - 变更原因和影响
- `tasks.md` - 实施任务清单
- `specs/[capability]/spec.md` - 规范变更（ADDED/MODIFIED）

**第三步：验证提案**
```bash
openspec validate add-new-feature --strict
```

### 4.3 规范文件格式

**ADDED Requirements（新增功能）**
```markdown
## ADDED Requirements

### Requirement: 新功能名称
系统 SHALL [行为描述]

#### Scenario: 成功场景
- **GIVEN** [前置条件]
- **WHEN** [触发条件]
- **THEN** [预期结果]
```

**MODIFIED Requirements（修改现有功能）**
```markdown
## MODIFIED Requirements

### Requirement: 现有功能名称
[完整复制原内容，然后修改需要变更的部分]

#### Scenario: [场景名称]
[场景内容]
```

**完整指南请查阅：** `openspec/AGENTS.md`（第3节：Creating Change Proposals）

---

## 五、核心接口速查

### 常用接口和类

| 功能 | 接口/类 | 命名空间 |
|------|---------|---------|
| 配置管理 | `IConfigurationService` | `BBMath.Configuration` |
| 文件操作 | `IFileService` | `BBMath.Core` |
| 日志记录 | `ILoggerService` | `BBMath.Core` |
| 题目生成 | `IMathProblemGenerator` | `BBMath.Core` |
| 答案验证 | `IProblemValidator` | `BBMath.Core` |
| 数学题目 | `MathProblem` | `BBMath.Core` |
| 游戏状态 | `GameStateManager` | `BBMath.Core` |

**使用示例：**
```csharp
// 读取配置
IConfigurationService config = new IniConfigurationService();
int coins = config.ReadInt("APP", "coinTtl");

// 记录日志
ILoggerService logger = new LoggerService();
logger.Info("应用程序启动");

// 生成题目（⚠️ 必须重用 Random 实例）
Random random = new Random();
IMathProblemGenerator generator = new MathProblemGenerator(random);
var problem = generator.GenerateProblem(ExamType.Addition, 3);

// 验证答案
IProblemValidator validator = new ProblemValidator();
bool isCorrect = validator.ValidateAnswer(problem, userAnswer);
```

**完整 API 文档请查阅：** `docs/API_REFERENCE.md`

---

## 六、搜索技巧

### 6.1 在文档中搜索

```bash
# 搜索 API 文档中的特定类
rg -n "class MathProblemGenerator" docs/API_REFERENCE.md

# 搜索所有文档中的关键词
rg -n "IConfigurationService" openspec/ docs/

# 搜索规范中的需求
rg -n "Requirement:|Scenario:" openspec/specs

# 搜索进行中的变更
rg -n "^#|Requirement:" openspec/changes
```

### 6.2 快速跳转

**已知类名/接口名 → API_REFERENCE.md**  
**已知功能模块 → specs/[capability]/spec.md**  
**不清楚时 → 本文档（场景化指引）**

---

**文档目的**：本文档是 AI 编程助手的快速参考指南，提供场景化查阅指引和核心约束速览。

**完整规范请查阅**：`openspec/project.md` 和 `openspec/AGENTS.md`

**API 详情请查阅**：`docs/API_REFERENCE.md`

**文档版本**: 2.1
**最后更新**: 2026-01-18
