## MODIFIED Requirements

### Requirement: 项目上下文文档
**原描述**：
项目上下文文档记录项目的用途、技术栈、约定、架构模式、测试策略、Git 工作流程、领域上下文、重要约束和外部依赖。

**修改后描述**：
项目上下文文档记录项目的用途、技术栈、约定、架构模式、测试策略、Git 工作流程、开发环境约束、领域上下文、重要约束和外部依赖。开发环境约束明确了 VS Code 与 VS 2019 IDE 的职责分工，规范了 AI 助手的操作限制和反馈机制。

#### Scenario: 查阅开发环境约束
- **GIVEN** AI 助手准备进行编程任务
- **WHEN** 查看 openspec/project.md
- **THEN**: 找到 Development Environment Constraints 章节，了解 VS Code、VS 2019 的职责分工和 AI 助手的禁止操作

#### Scenario: 需要修改窗体控件
- **GIVEN** AI 助手需要添加新的按钮控件
- **WHEN** 查看开发环境约束
- **THEN**: 发现禁止修改 Designer.cs 文件的规定，按照流程提供手动操作说明并等待用户确认

#### Scenario: 需要编译项目
- **GIVEN** AI 助手完成代码编写
- **WHEN** 准备测试代码
- **THEN**: 根据约束提醒用户在 VS 2019 中编译，并要求反馈编译结果
