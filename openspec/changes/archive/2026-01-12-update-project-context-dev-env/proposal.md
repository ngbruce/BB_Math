# Change: 更新项目上下文文档，补充开发环境约束

## Why
在进行项目重构前，需要明确开发环境的工作分工：
1. VS Code 环境仅用于代码编写和文档维护，不能编译和调试 C# 工程
2. VS 2019 IDE 负责所有编译、调试、运行工作
3. AI 助手禁止修改窗体设计文件（*.Designer.cs）
4. 需要建立清晰的反馈机制，确保 AI 助手能及时获得编译错误和运行时异常信息

这些约束对于规范 AI 助手的编程行为、避免错误操作至关重要。

## What Changes
- **补充 project.md**：新增 "Development Environment Constraints" 章节
  - 明确 VS Code 与 VS 2019 的职责分工
  - 定义 AI 助手的禁止操作和限制
  - 建立窗体/控件修改的标准流程
  - 规范编译调试的反馈机制

## Impact
- **Affected specs**:
  - `project-management/project-context` - 项目上下文文档
  
- **Affected code**: 无代码修改，仅文档更新

- **Breaking changes**: 无

- **Migration required**: 无