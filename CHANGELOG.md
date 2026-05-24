# 更新日志

本项目的所有重要更改都记录在此文件中。

格式基于 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.0.0/)，
本项目遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

## [7.1.4] - 2026-05-24

### 新增
- ExamTypePool.AddToType() 方法：答错惩罚题目直接追加到错题型池
- 题型日志输出"总数"字段（SumQty），便于查看各题型答题量
- 余数输入框 tbRemainder 支持 KeyDown 事件，与主输入框行为一致

### 修复
- 禁用进程黑名单监控定时器（timer2）：Process.GetProcesses() 枚举全部进程会卡 UI 线程

### 改进
- 答题入口统一：button2_Click 直接调用 `_presenter.ValidateAnswer()`，废弃遗留的 `answer()` 方法
- `answer()` 方法标记 `[Obsolete("答题逻辑已迁移到 MainFormPresenter.ValidateAnswer()")]`

### 文档
- 删除冗余文档：移除 `docs/API_REFERENCE.md`（源代码 XML 注释已覆盖）、`docs/TESTING.md`（README 已覆盖）、`docs/CODING_STANDARDS.md`（已合并到 config.yaml）
- 合并开发规范：将 CODING_STANDARDS 中的场景导航表、窗体修改流程、额外禁止事项迁移到 `openspec/config.yaml`
- 简化 `openspec/project.md` 为项目上下文重定向页
- 清理已归档变更的旧文件（refactor-mvp-architecture 旧路径）
- 更新 README 文档引用和项目结构树
- 更新 PR 模板编码规范链接至 `openspec/config.yaml`

### 移除
- 移除全局 `AGENTS.md` 和 `openspec/AGENTS.md`（项目上下文统一在 config.yaml 管理）

## [7.1.3] - 2026-01-21

### 改进
- 优化密码输入窗口界面：简化布局，移除冗余控件
- 添加默认密码说明提示，用户可以在《使用说明.pdf》中查看初始密码
- 更新版本号至 7.1.3

## [7.1.2] - 2026-01-21

### 新增
- 在"原作说明"页面添加淘宝商品链接，点击可直接跳转到商品页面

### 改进
- 优化 README 文档，补充功能说明和使用细节
- 添加 gitee 镜像地址说明，方便国内用户访问
- 完善数据备份和卸载说明
- 更新版本号至 7.1.2


## [7.1.1] - 2026-01-18

### 修复
- 修复所有难度的随机数生成器重复初始化问题：将 MathProblemGenerator 改为类成员变量，重用 Random 实例，避免出现相同题目重复生成的错误
- 修复定时器重置问题：每题生成后正确重置倒计时为60秒，支持所有难度和题型
- 修复除法 intBits 计算错误：
  - LV3 难度：修复除数和商总是相同的bug（DivisionNoRemainderStrategy 的 max 计算公式错误）
  - LV4 难度：修复被除数 >= 100 导致的无限重试问题，调整为使用个位数除法（effectiveIntBits=1）

---

## [7.1.0] - 2026-01-17

### 新增
- 日志目录自动创建：日志文件存储在 `log` 子目录下，便于管理
- 初始化错误处理增强：每个初始化步骤失败时详细记录日志、显示友好错误提示并终止程序
- 窗体标题显示版本号：启动时自动从 AssemblyVersion 读取版本号并显示在标题栏

### 修复
- 修复 Application.Exit() 在构造函数中不生效的问题，改用 Environment.Exit(1)
- 修复 Program.cs 和 Form1.cs 中双重初始化日志的问题

### 改进
- 优化日志文件路径管理，使用统一的 `log` 目录存储所有日志文件
- 强化 Presenter 初始化错误处理，确保关键组件初始化失败时程序立即终止
- 移除所有冗余的 `_presenter` null 检查，简化代码逻辑

### 移除
- 废弃 `GenNum()` 方法（使用 Obsolete 特性标记），题目生成完全由 Presenter 负责

### 文档
- 更新 README.md 日志说明，反映新的日志目录结构
- 更新开发规范，明确常量管理要求
- **重大更新**：更换为 BB Math Custom Public License v1.0（基于 MPL 2.0 + 商业限制）
- **重大更新**：文档整合，将 CONTRIBUTING、CODE_OF_CONDUCT、SUPPORT、SECURITY 内容合并到 README
- 添加完整的开源文档体系，包括 LICENSE、README、CHANGELOG 等

## [7.0.0] - 2026-01-16

### 新增
- MVP 架构重构，实现界面与业务逻辑分离
- OpenSpec 规范管理体系，规范化功能开发流程
- 5个难度级别选择功能（LV1-LV5）
- 题型分布平衡优化，确保各题型均匀分布
- 测试日志打印答案功能，方便调试
- 完整的单元测试覆盖（219个测试，全部通过）

### 修复
- 修复 LV5 难度出题重试到顶的问题
- 修复暂停机制配置值与运行时值分离问题
- 修复测试失败问题：
  - `TotalQuestionTypes_ShouldBePositive` 期望值更新为 5
  - `CheckAndFixGameState` 测试使用配置值而非运行时值
- 修复配置文件缺失时金币默认值错误（从50改为10）

### 改进
- 优化全对奖励金币计算逻辑（初始题数 × 0.5）
- 修正除法策略使用 intBits 参数，避免硬编码个位数
- 完善错误恢复服务，自动检测和修复配置文件损坏

### 文档
- 添加完整的 API 参考文档
- 添加开发规范和编码标准
- 添加 OpenSpec 使用指南
- 添加详细的 README 文档

## [6.x.x] - 2025-xx-xx

### 新增
- 基础数学练习功能
- 金币奖励系统
- 暂停机制
- 日志记录功能

### 修复
- 修复配置文件损坏问题
- 修复金币保存问题

---

## 版本说明

### 版本号格式：主版本号.次版本号.修订号

- **主版本号**：不兼容的 API 修改
- **次版本号**：向下兼容的功能性新增
- **修订号**：向下兼容的问题修正

### 变更类型

- **新增**：新功能
- **修复**：bug 修复
- **改进**：现有功能改进
- **移除**：功能移除
- **文档**：文档相关更改
