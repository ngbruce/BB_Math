# Change: 项目复核与代码结构规范化重构

## Why
当前项目经过多年开发迭代，积累了大量技术债务。代码结构存在以下问题：
1. 命名空间 `WindowsFormsApplication3` 未反映项目实际用途
2. 部分类职责不清（如 `bbMath` 静态类包含过多功能）
3. 缺乏统一的错误处理机制
4. 配置文件管理耦合度高
5. 缺少自动化测试覆盖
6. 代码注释和文档不完整
7. **存在大量硬编码的魔法数字和字符串**（如密码默认值、允许暂停时长、配置文件名等），分散在代码各处，难以维护和修改

通过规范化重构，提升代码可维护性、可测试性和可扩展性，为后续功能开发奠定基础。

## What Changes
- **重命名命名空间**：从 `WindowsFormsApplication3` 改为 `BBMath.Application`
- **重构静态类**：将 `bbMath` 拆分为 `ConfigurationManager`、`GameStateManager`、`FileManager` 等专用类
- **提取接口**：为数据访问、配置管理等提取接口，便于测试和扩展
- **统一异常处理**：建立全局异常处理机制
- **配置系统重构**：封装配置读写逻辑，提供强类型配置访问
- **日志系统改进**：实现结构化日志记录
- **添加单元测试**：为核心业务逻辑添加单元测试（使用 MSTest 或 NUnit）
- **代码注释完善**：补充 XML 文档注释，提高代码可读性
- **文件命名规范化**：统一文件命名约定
- **常量规范化**：创建统一常量类 `AppConstants`，集中管理项目中的硬编码值（如默认密码、暂停时长、文件名等）
- **枚举类型规范化**：将 `PauseTypeByCount`、`PauseTypeByTime` 等整型常量转换为枚举类型，提升代码可读性和类型安全性
- **配置项验证**：为配置项添加合理的范围验证，当配置值超出范围时记录警告并使用默认值

## Impact
- **Affected specs**:
  - `core/math-operations` - 数学运算核心逻辑
  - `configuration-management` - 配置管理系统
  - `file-io` - 文件读写操作
  - `game-state` - 游戏状态管理
  - `logging` - 日志记录功能
  - `core/constants` - 常量定义（新增）

- **Affected code**:
  - `Program.cs` - 命名空间变更
  - `Form1.cs` - 引用更新
  - `bbMath` 静态类 - 重构拆分
  - `Ini_File.cs` - 封装改进
  - 所有表单文件 - 命名空间更新
  - 多个包含硬编码常量的文件 - 替换为引用 `AppConstants`

- **Breaking changes**:
  - 命名空间变更需要更新所有 using 语句
  - 配置文件格式可能调整
  - 部分公共 API 接口变更

- **Migration required**:
  - 升级后需要测试现有配置文件的兼容性
  - 需要验证所有功能模块正常工作
