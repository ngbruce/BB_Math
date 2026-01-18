# Task Checklist: 题目难度选择功能

## 1. 核心数据结构
- [x] 1.1 在 Core 命名空间中创建 Difficulty 枚举 (Easy, Medium, Hard)
- [x] 1.2 创建 DifficultyConfiguration 类,定义各难度的参数配置
  - 数值范围 (最小值/最大值)
  - 支持的运算类型列表
  - 整数位数
  - 其他参数 (是否允许负数等)

## 2. 题目生成逻辑修改
- [x] 2.1 修改 MathProblemGenerator 或相关策略,支持根据难度生成题目
- [x] 2.2 实现难度到题目参数的映射逻辑
- [x] 2.3 确保与现有策略模式 (AdditionStrategy 等) 兼容
- [x] 2.4 修改 MainFormPresenter.GenerateProblem(),根据难度配置生成题目
- [x] 2.5 实现 Easy/Medium 难度的结果<100检查
- [x] 2.6 实现 Hard 难度的至少一个数>=100检查

## 3. 记录数据结构修改
- [x] 3.1 在 PracticeRecord 类中添加 Difficulty 属性
- [x] 3.2 修改 ToCsvString 方法,在 CSV 输出中包含难度信息
- [x] 3.3 修改 Form1.cs 中的 save 方法,在日志文件输出中包含难度信息

## 4. UI 交互
- [x] 4.1 为 cbDifficultySelect 控件添加 SelectedIndexChanged 事件处理函数
- [x] 4.2 在事件处理中保存当前选择的难度到 GameStateManager 或相关状态管理类
- [x] 4.3 在题目生成前读取难度配置,设置对应的参数
- [x] 4.4 设置默认难度为 Easy,在窗体加载时选中

## 5. 状态管理
- [x] 5.1 在 GameStateManager 中添加当前难度状态字段
- [x] 5.2 确保难度状态在答题开始时正确初始化

## 6. 重新开始练习功能
- [x] 6.1 在答题完成后显示 btnDoAgain 按钮 (answer 方法中)
- [x] 6.2 实现 btnDoAgain_Click 事件处理函数:
  - 恢复初始状态 (解锁开始按钮、重置界面)
  - 允许用户重新输入题目数量
  - 隐藏 btnDoAgain 按钮
- [x] 6.3 确保开始答题时 btnDoAgain 按钮为不可见

## 7. 测试与验证
- [x] 7.1 测试三种难度下的题目生成是否符合预期
- [x] 7.2 测试记录文件是否正确包含难度信息
- [x] 7.3 测试难度切换功能是否正常工作
- [x] 7.4 测试重新开始按钮功能:
  - 答题完成后按钮是否显示
  - 点击后是否恢复初始状态
  - 再次答题后按钮是否隐藏
- [x] 7.5 验证向后兼容性 (默认行为)
