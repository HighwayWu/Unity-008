# Unity-008

# Unity之万丈高楼第八砖

一个双人对战的游戏

跟着教程使用socket在vs2015上做了个小的服务器 跟之前聊天小程序的差不多 额外加了好多用来处理不同信息的功能 能连接MySQL数据库 将玩家的信息、战绩等进行保存

服务器端:

Common(同客户端共用统一一套code来区别消息的传递):
	RequestCode: 来区别request要交给哪个controller处理 如RoomController处理关于创建房间 GameController处理关于人物角色
	ActionCode: controller进行具体的处理方式 如room之中可选择create join update quit等
	
Server:
	Model: 保存了数据库中表的结构
	DAO: 提供MySqlConnection之后可以从数据库中获取数据
	Controller: 对服务器端、客户端之间传递的消息进行处理
	Server: 创建serverSocket 用于接收客户端传递的消息 并将消息传递给controller进行处理 处理完成后将结果传递给client
	Client: 创建clientSocket 用于接收serverSocket的消息 并将其传递给客户端
	
客户端:

GameFacade: 单例模式 主要用于处理各个模块之间的信息交互 如接收到clientManager的消息后交给requestManager进行处理
ClientManager: 创建socket 接收服务器端发送的消息 转交给gameFacade进行处理 或从gameFacade处接收消息并发送给服务器端
RequestManager: 根据不同的actionCode处理消息 或者根据不同的情况发送带有actionCode的消息给gameFacade
其余...Manager: 基本上就是根据其名字对响应模块进行处理

画了个大致的结构图:

![image](https://github.com/HighwayWu/Unity-008/raw/master/Screenshots/Structure.png)

像这种稍微大一点的东西做起来不先画个图非得把自己绕晕不可。。貌似突然明白了架构师是多么得重要。。耦合松了不方便调用 耦合紧了改动又太麻烦。。

开始游戏跟登录界面:

![image](https://github.com/HighwayWu/Unity-008/raw/master/Screenshots/图片1.png)

注册界面跟登录成功后:

![image](https://github.com/HighwayWu/Unity-008/raw/master/Screenshots/图片2.png)

创建房间以及第二名玩家的加入 然后房主就可以开始游戏啦:

![image](https://github.com/HighwayWu/Unity-008/raw/master/Screenshots/图片3.png)

游戏界面跟结束界面:

![image](https://github.com/HighwayWu/Unity-008/raw/master/Screenshots/图片4.png)

另外提一句。。DOTween做各个按钮、界面的出现、消失等动画是真的方便 想改参数在代码里面直接就能改 不用再去动画界面了

数据库的两个表 user(用户名密码)跟result(保存战绩)

![image](https://github.com/HighwayWu/Unity-008/raw/master/Screenshots/图片5.png)

也算是一个稍微大一点的游戏了诶。。但是好几个文件上传不了。。Assets里是Unity的素材跟脚本 Server_vs2015里是服务器端的程序

感觉离梦想又近了一点点

路漫漫其修远兮~
