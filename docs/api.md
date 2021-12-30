# 接口文档

- 标题前的序号代表接口标识号

## 通用请求参数

通用请求参数会追加在接口所需请求参数之后，需要传递如下参数：

| 参数           | 说明                                                         | 备注 |
| -------------- | ------------------------------------------------------------ | ---- |
| platformSource | 默认为10                                                     | 必选 |
| fordown        | 默认值1                                                      | 必选 |
| downver        | 客户端版本号                                                 | 必选 |
| downuid        | SsoUid                                                       | 必选 |
| _r             | 当前时间的滴答数（自公历1月1日午夜12:00:00起），1个滴答代表100纳秒（千万分之一秒） | 必选 |
| _t             | 当前时间的滴答数（自公历1月1日午夜12:00:00起），1个滴答代表100纳秒（千万分之一秒） | 必选 |

## 通用接口

### 10001 获取服务器时间

接口请求地址为：

```http
POST http://portal.cdeledu.com/interface/ucGetSTime.php
```

返回的信息如下：

```none
1618990280
```

### 10002 上传用户操作记录

接口的请求地址为：

```http
POST http://data.cdeledu.com/SaveFileData
```

需要传递如下参数：

| 字段       | 说明                                          | 备注 |
| ---------- | --------------------------------------------- | ---- |
| userID     | 用户ID                                        | 必选 |
| siteID     | 数据采集站点ID                                | 必选 |
| deviceId   | 设备序列号                                    | 必选 |
| file       | data.log文本内容                              | 必选 |
| submitTime | 提交时间                                      | 必选 |
| pkey       | Ssouid、time、cdelofflineClient加密而来的密钥 | 必选 |

返回信息如下：

```none
true
```

> ture为成功，false为失败

## 会计下载课堂

### 10101 客户端更新

通过该接口可以更新客户端，接口请求地址为：

```http
GET http://game.chinaacc.com/CourseClientUpdate/Chinaacc/version.xml
```

返回数据包含如下字段：

| 字段名     | 说明                   | 备注 |
| ---------- | ---------------------- | ---- |
| ver        | 当前版本号             |      |
| infos      | （未使用）             |      |
| infos/info | （未使用）             |      |
| files      | 版本更新涉及的文件合集 |      |
| files/file | 版本更新涉及的文件     |      |

file字段包含如下属性：

| 属性名    | 说明       |
| --------- | ---------- |
| filename  | 文件名     |
| localpath | 本地路径   |
| remoteurl | 下载路径   |
| updatecmd | 更新命令   |
| ver       | 文件版本   |
| filehash  | 文件哈希值 |

XML格式的返回信息如下：

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <ver>1.1.2.2</ver>
  <infos>
    <info ver="1.0.0.10">
    </info>
  </infos>
  <files>
    <file filename="CdelCourse.exe" localpath="\CdelCourse.exe" remoteurl="http://game.chinaacc.com/CourseClientUpdate/Chinaacc/CdelCourse.exe" updatecmd="" ver="1.1.2.2" filehash="CCB5DB5B071388DB93234D9A59F1B31384482679" />
    <file filename="Framework.dll" localpath="\Framework.dll" remoteurl="http://game.chinaacc.com/CourseClientUpdate/Chinaacc/Framework.dll" updatecmd="" ver="1.1.2.2" filehash="9FF4A06B56A8172644C3556DFED942E7EA2D2BBC" />
    <file filename="GalaSoft.MvvmLight.dll" localpath="\GalaSoft.MvvmLight.dll" remoteurl="http://game.chinaacc.com/CourseClientUpdate/Chinaacc/GalaSoft.MvvmLight.dll" updatecmd="" ver="1.1.0.88" filehash="" />
    <file filename="GalaSoft.MvvmLight.Extras.dll" localpath="\GalaSoft.MvvmLight.Extras.dll" remoteurl="http://game.chinaacc.com/CourseClientUpdate/Chinaacc/GalaSoft.MvvmLight.Extras.dll" updatecmd="" ver="1.1.0.88" filehash="" />
  </files>
</root>
```

### 10102 获取学员跑马灯信息

通过该接口可以从服务器获取在客户端的底部需要显示的滚动信息，接口请求地址为：

```http
POST http://manage.mobile.cdeledu.com/analysisApi/dlclassroom/getMarqueeInfoList.shtm
```

需要传递以下参数：

| 参数           | 说明   | 备注 |
| -------------- | ------ | ---- |
| pkey           | md5校验码 | 使用以下字段计算得到：<br />appID、<br />platformSource、<br />time+"eiiskdui" |
| appID          | 程序id | 用于区分“会计课堂”、“建筑课堂”、“医学课堂”等 |
| time           | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss |

返回数据包含如下字段：

| 字段名          | 说明                     | 备注 |
| --------------- | ------------------------ | ---- |
| code            | 状态指示（暂未使用）     |      |
| marqueeInfoList | 跑马灯需要展示的具体信息 |      |
| msg             | 返回的信息               |      |

marqueeInfoList字段包含如下字段：

| 属性名      | 说明               |
| ----------- | ------------------ |
| appKey      | （暂未使用）       |
| appName     | （暂未使用）       |
| courseEduID | （暂未使用）       |
| fontColour  | 使用的字体颜色     |
| id          | （暂未使用）       |
| operator    | （暂未使用）       |
| platformId  | （暂未使用）       |
| pushContent | 推送的具体信息内容 |
| pushType    | （暂未使用）       |
| realName    | （暂未使用）       |
| rowNumEnd   | （暂未使用）       |
| rowNumStart | （暂未使用）       |
| siteId      | （暂未使用）       |
| siteName    | （暂未使用）       |

JSON格式的返回信息如下：

```json
{
    "marqueeInfoList": [
        {
            "appKey": "",
            "appName": "",
            "courseEduID": "",
            "fontColour": "",
            "id": 1,
            "operator": 36,
            "platformId": 2,
            "pushContent": "关课提示：下载课程在考试结束后一周关闭，请抓紧时间学习！（会计从业课程关闭时间以招生方案公布的内容为准！）",
            "pushType": 1,
            "realName": "",
            "rowNumEnd": 0,
            "rowNumStart": 0,
            "siteId": 1,
            "siteName": ""
        }
    ],
    "code": 1,
    "msg": "成功"
}
```

### 客户端认证

#### 10201 账户强制下线检测

通过该接口可以检测是否有其它终端登录而导致本地账户需要强制下线，接口请求地址为：

```http
POST http://portal.cdeledu.com/api/index.php
```

需要传递以下参数：

| 参数    | 说明   | 备注 |
| ------- | ------ | ---- |
| cmd     | 需要执行的命令 | 固定字符串"ucChkUserLogin" |
| sid     | 当前用户唯一识别码 |      |
| ssouid  | SsoUid |      |
| selfsid | （暂不明确） | 固定字符串“0” |
| pkey    | md5校验码 | 通过以下字段计算得到：<br />固定字符串"fJ3UjIFyTu"（用于加盐）、<br />time、<br />cmd、<br />sid、<br />ssouid |
| time    | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss |

返回数据包含如下字段：

| 字段名 | 说明         |
| ------ | ------------ |
| ret    | 返回的数据体 |

ret字段包含如下字段：

| 属性名 | 说明       | 备注                                         |
| ------ | ---------- | -------------------------------------------- |
| code   | 状态指示   | “-7”：账户已在另一地点登录，本地需要强制下线 |
| msg    | 返回的消息 |                                              |

XML格式的返回信息如下：

```xml
<ret>
<code>0</code>
<msg>user online</msg>
</ret>
```

#### 10202 账户冻结检测

通过该接口可以检测当前账户是否被冻结，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/versionm/classroom/member/getUsserWaring
```

返回数据包含如下字段：

| 字段名         | 说明 | 备注 |
| -------------- | ---- | ---- |
| ltime          | token的令牌时间 |      |
| pkey           | md5校验码 | 通过以下字段计算得到：<br />userID、<br />platformSource、<br />version、<br />time、<br />固定字符串"fJ3UjIFyTu"（用于加盐）、<br />token值 |
| time           | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss |
| userID         | SsoUid |      |
| version        | 程序版本号 |      |

返回数据包含如下字段：

| 字段名 | 说明       | 备注                        |
| ------ | ---------- | --------------------------- |
| code   | 状态指示   | “1”：表示账户异常，已被冻结 |
| msg    | 返回的信息 |                             |

JSON格式的返回信息如下：

```json
{
    "code": 0,
    "msg": "连接异常，请与客服联系!"
}
```

#### 10203 用户登录

通过该接口可以登录客户端账号，接口请求地址为：

```http
POST http://portal.cdeledu.com/interface/login.php
```

需要传递以下参数：

| 参数        | 说明                        | 备注                                                         |
| ----------- | --------------------------- | ------------------------------------------------------------ |
| username    | 登录账号                    |                                                              |
| domain      | 域名                        |                                                              |
| passwd      | 登录密码                    |                                                              |
| memberlevel | （暂未明确）                | 固定字符串“pc”                                               |
| memberkey   | （暂未明确）                | 固定字符串"84772CDB"                                         |
| pkey        | md5校验码                   | 使用下列字段计算得到：<br />username、<br />domain、<br />passwd、<br />memberlevel、<br />固定字符串"eiiskdui"（用于加盐） |
| mid         | 设备加密密钥（机器key）     | 数据库表DBKey中的MainKey，或注册表中的硬件信息               |
| mname       | 本地计算机主机名            |                                                              |
| appname     | 当前登录的程序名            | 用于区分“会计课堂”、“建筑课堂”、“医学课堂”等                 |
| version     | 程序版本号                  |                                                              |
| from        | （暂未明确）                | 固定字符串"member_app_passwd"                                |
| desc        | 同appname，当前登录的程序名 |                                                              |

返回数据包含如下字段：

| 字段名 | 说明               | 备注                                                         |
| ------ | ------------------ | ------------------------------------------------------------ |
| code   | 登录操作状态指示   | “0”：用户名密码正确<br />“-4”：学员代码或密码错误<br />“-5”：学员代码或密码错误<br />“-12”：在太多电脑上登录了该帐号<br />“-18”：该设备已经被注销，不能再次使用<br />其它代码：登录失败<br /> |
| msg    | 返回的指示信息     |                                                              |
| ssouid | SsoUid             |                                                              |
| sid    | 当前用户唯一识别码 |                                                              |
| ver    | （暂未明确）       |                                                              |

XML格式的返回信息如下：

```xml
<ret>
<code>0</code>
<msg>success</msg>
<ssouid>33049216</ssouid>
<sid>911542d6-f8d3-4a82-83cc-a9b3febf7994</sid>
<ver>1</ver>
</ret>
```

#### 10204 上传登录记录

通过该接口可以记录用户在客户端上的登录操作，接口请求地址为：

```http
POST http://mportal.chinaacc.com/video/header/Visitor
```

需要传递以下参数：

| 参数      | 说明   | 备注 |
| --------- | ------ | ---- |
| userName  | 登录账户名 |      |
| ssoUid    | SsoUid |      |
| osVersion | 操作系统版本 |      |
| ssid      | 设备加密密钥（机器key） | 数据库表DBKey中的MainKey，或注册表中的硬件信息 |
返回信息如下：

```none
Y
```

#### 10205 获取Token访问令牌

通过该接口可以获取token访问令牌，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/auth/token/getToken
```

需要传递以下参数：

| 参数    | 说明             | 备注                                                         |
| ------- | ---------------- | ------------------------------------------------------------ |
| appkey  | 程序key          | 区分程序类别（会计课堂、医学网、建设网……）                   |
| pkey    | md5校验码        | 使用下列字段计算得到：<br />platformSource、<br />version、<br />time、<br />固定字符串"Nyjh5AEeMw"（用于加盐）、<br />appkey |
| time    | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss                                      |
| version | （暂未明确）     | 固定字符串"2.0.0.0"                                          |

返回数据包含如下字段：

| 字段名     | 说明                  | 备注                                                         |
| ---------- | --------------------- | ------------------------------------------------------------ |
| code       | 状态指示              | ”0“：获取失败<br />”1“：获取成功                             |
| paramValue | 包含token信息的加密串 | 使用电子密码本 (ECB) 模式解密，可得到三个字段<br />"token":"d36602275d"<br />"longtime":"1619344161114"<br />"timeout":86400 |

paramValue字段使用”电子密码本 (ECB) “模式解密后包含如下字段：

| 属性名   | 说明         |
| -------- | ------------ |
| token    | 访问令牌的值 |
| longtime | 令牌时间     |
| timeout  | 有效期       |

JSON格式的返回信息如下：

```json
{
    "code": 1,
    "paramValue": "aU5c1fqNF-CnnNbqckzzKv7uQKs0u1RyDLJQxWl9h43CDTGsG-02ZJqdkQbxGJHrecawKLMc7JdTgp4R9kV57LA2hahO0QQ8"
}
```

### 10301 同步学习记录

通过该接口可以同步客户端上的学习记录，登录成功后会调用一次，接口请求地址为：

```http
POST http://member.chinaacc.com/mapi/classroom/versionm/record/saveBatchMessage
```

需要传递以下参数：

| 参数 | 说明   | 备注 |
| ---- | ------ | ---- |
| appkey | 程序key | 用于区分“会计课堂”、“建筑课堂”、“医学课堂”…… |
| guid | 全局唯一标识符 |      |
| ltime | token的令牌时间 |      |
| online | 是否在线标识 | “1”：表示在线<br />“0”：表示离线 |
| pkey | md5校验码 | 通过以下字段计算得到：<br />SsoUid、<br />platformSource、<br />version、<br />time、<br />type、<br />guid、<br />固定字符串"fJ3UjIFyTu"（用于加盐）<br />token值 |
| studyVideoJson | 学习记录信息 |      |
| time | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss |
| type | （暂不明确） | 固定字符串“qz” |
| uid | SsoUid | |
| userID | SsoUid | |
| version | 程序版本号 | |

studyVideoJson字段包含如下字段：

| 参数     | 说明         | 备注 |
| -------- | ------------ | ---- |
| videoStr | 视频信息详情 |      |

videoStr字段包含如下字段：

| 参数       | 说明             | 备注 |
| ---------- | ---------------- | ---- |
| cwareID    | 课件ID           |      |
| deviceID   | 设备ID           |      |
| rangeEnd   | 视频总长度结束点 |      |
| rangeStart | 视频总长度开始点 |      |
| videoID    | 视频ID           |      |
| timebase   | 课件学习详情     |      |

timebase字段包含如下字段：

| 参数           | 说明           | 备注 |
| -------------- | -------------- | ---- |
| CwareId        | 课件ID         |      |
| VideoID        | 视频ID         |      |
| VideoStartTime | 视频开始时间点 |      |
| VideoEndTime   | 视频结束时间点 |      |
| Speed          | 倍速           |      |
| StudyTimeEnd   | 学习结束时间   |      |
| StudyTimeStart | 学习开始时间   |      |

返回数据包含如下字段：

| 字段名 | 说明       | 备注                                     |
| ------ | ---------- | ---------------------------------------- |
| code   | 状态指示   | “1”：表示同步成功<br />“0”：表示同步失败 |
| msg    | 返回的信息 |                                          |

JSON格式的返回信息如下：

```json
{
    "code": 1,
    "msg": "成功"
}
```

### 我的课程

#### 10401 获取班次类别列表

通过该接口可以获取班次类别列表，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/versionm/classroom/mycware/getUserCourseList
```

需要传递以下参数：

| 参数    | 说明                                  | 备注                                                         |
| ------- | ------------------------------------- | ------------------------------------------------------------ |
| appkey  | 程序key                               | 用于区分“会计课堂”、“建筑课堂”、“医学课堂”……                 |
| ltime   | token的令牌时间                       |                                                              |
| pkey    | md5校验码                             | 使用下列字段计算得到：<br />sid、<br />platformSource、<br />version、<br />time、<br />固定字符串"fJ3UjIFyTu"（用于加盐）、<br />token值 |
| sid     | 当前用户唯一识别码                    |                                                              |
| time    | 带格式的当前时间，yyyy-mm-dd+hh:mm:ss |                                                              |
| version | 程序版本号                            |                                                              |

返回数据包含如下字段：

| 字段名               | 说明         |
| -------------------- | ------------ |
| uid                  | （未使用）   |
| code                 | （未使用）   |
| myCourseInfo         | 课程信息     |
| subectCourseRelation | 课程相关信息 |

myCourseInfo字段包含如下字段：

| 属性名         | 说明         |
| -------------- | ------------ |
| boardID        |              |
| courseEduID    |              |
| courseEduName  |              |
| dispOrder      |              |
| eduSubjectID   | 班次类别id   |
| eduSubjectName | 班次类别名称 |
| orderNo        |              |

subectCourseRelation字段包含如下字段：

| 属性名       | 说明       |
| ------------ | ---------- |
| courseID     |            |
| eduSubjectID | 班次类别id |

JSON格式的返回信息如下：

```json
{
    "uid": 25441020,
    "code": "1",
    "myCourseInfo": [
        {
            "boardID": 272,
            "eduSubjectName": "会计",
            "orderNo": 210,
            "eduSubjectID": 3070,
            "courseEduName": "注册会计师",
            "courseEduID": 8,
            "dispOrder": 372
        }
    ],
    "subectCourseRelation": [
        {
            "eduSubjectID": 2955,
            "courseID": "acc1214230"
        }
    ]
}
```

#### 10402 获取班次列表

通过该接口可以获取班次类别列表，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID
```

需要传递以下参数：

| 参数         | 说明               | 备注                                                         |
| ------------ | ------------------ | ------------------------------------------------------------ |
| eduSubjectID | 课程类别id         |                                                              |
| ltime        | token的令牌时间    |                                                              |
| pkey         | md5校验码          | 涉及sid、eduSubjectID、platformSource、version、固定字符串"fJ3UjIFyTu"加盐、token值、time |
| sid          | 当前用户唯一识别码 |                                                              |
| time         | 带格式的当前时间   | 格式yyyy-mm-dd+hh:mm:ss                                      |
| uid          | SsoUid             |                                                              |
| version      | 程序版本号         |                                                              |
| vflag        | （暂不明确）       | 可选                                                         |

返回数据包含如下字段：

| 字段名       | 说明                       |
| ------------ | -------------------------- |
| code         | 状态指示（暂未使用）       |
| cwList       | 班次列表信息               |
| edusubjectID | 班次类别id（暂未使用）     |
| imsg         | 消息返回（暂未使用）       |
| msg          | 消息返回（暂未使用）       |
| userID       | （暂未使用）               |
| videoList    | 班次的视频信息（暂未使用） |

cwList字段包含如下字段：

| 属性名           | 说明                                   |
| ---------------- | -------------------------------------- |
| boardID          | 答疑板id                               |
| classOrder       | 班次排序                               |
| classOrder1      |                                        |
| cwareClassID     | 课件班次id                             |
| cwareClassName   | 课件班次名称                           |
| cwareID          | 课件id（例如：37076）                  |
| cwareImg         | 课件图片                               |
| cwareName        | 课件名称                               |
| cwareTitle       | 课件标题                               |
| cwareUrl         | 课件地址                               |
| cwID             | 课件id（例如：acc121541b）             |
| cYearName        | 课程年份                               |
| dateEnd          | 关课时间                               |
| datNum           |                                        |
| downLoad         | 是否可下载                             |
| eduOrder         |                                        |
| highClass        |                                        |
| homeShowYear     |                                        |
| isFree           |                                        |
| isMobileClass    | 是否移动端课程（移动端功能已移除）     |
| mobileCourseOpen | 是否开通移动端课程（移动端功能已移除） |
| mobileTitle      | 移动端显示标题（移动端功能已移除）     |
| openTime         | 开通时间                               |
| projectID        |                                        |
| prompt           | 课件提示                               |
| queNum           |                                        |
| rownum           |                                        |
| specialFlag      |                                        |
| teacherName      | 老师名称                               |
| updateTime       | 更新时间                               |
| useFul           |                                        |
| videoType        | 视频类型                               |
| wareOrder        | 课件排序                               |
| yearOrder        | 年份排序                               |

videoList字段包含如下字段：

| 属性名       | 说明     |
| ------------ | -------- |
| cwid         | 课件id   |
| thisProgress | 视频进度 |

JSON格式的返回信息如下：

```json
{
    "userID": "25441020",
    "cwList": [
        {
            "isMobileClass": 0,
            "boardID": 0,
            "datNum": 0,
            "mobileTitle": "零基础预习[超值精品]",
            "cwareName": "【零基础预习[超值精品]】会计-刘国峰（2021年）",
            "dateEnd": "2021-12-31 00:00:00",
            "openTime": "2020-11-01 15:27:00",
            "classOrder": 11,
            "cwareClassName": "零基础预习[超值精品]",
            "teacherName": "刘国峰",
            "cYearName": "2021年",
            "rownum": 1,
            "projectID": 0,
            "cwID": "acc131541b",
            "highClass": "0",
            "specialFlag": 1,
            "cwareClassID": 574,
            "eduOrder": 210,
            "homeShowYear": 1,
            "cwareUrl": "http://elearning.chinaacc.com/xcware/video/videoList/videoList.shtm?cwareID=37076",
            "cwareImg": "https://img.cdeledu.com/CWARE/2020/1103/a97f8a5df0e8da0a-0.png",
            "queNum": 0,
            "courseOpenExplain": "您所报的课程费用已交纳，课程将于上一年考试结束一周后陆续开通，敬请关注！",
            "updateTime": "2020-11-03 16:36:00",
            "wareOrder": 21019,
            "classOrder1": 11,
            "mobileCourseOpen": 1,
            "videoType": 1,
            "cwareID": 37076,
            "yearOrder": 88,
            "isFree": 0,
            "cwareTitle": "零基础预习[超值精品]",
            "downLoad": "Y",
            "prompt": "上一年考试结束一周后",
            "useFul": 1
        }
    ],
    "imsg": "成功",
    "code": 1,
    "edusubjectID": "3070",
    "msg": "成功",
    "videoList": [
        {
            "thisProgress": 43.17,
            "cwid": "acc131541b"
        }
    ]
}
```

#### 10403 获取班次内的章节、讲次信息

通过该接口可以获取班次内的章节和讲次的具体信息，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/versionm/classroom/course/getCourseDetail
```

需要传递以下参数：

| 参数            | 说明             | 备注                                                         |
| --------------- | ---------------- | ------------------------------------------------------------ |
| cwID            | 课件id           |                                                              |
| freeOpenVersion | 课件id，同cwID   |                                                              |
| getType         | （暂不明确）     | 固定字符串”2“                                                |
| innerCwareID    | （暂不明确）     | 可选                                                         |
| ltime           | token的令牌时间  |                                                              |
| pkey            | md5校验码        | 使用下列字段计算得到：<br />getType、<br />username、<br />classid、<br />platformSource、<br />version、<br />time、<br />token值、<br />固定字符串"fJ3UjIFyTu"（用于加盐） |
| platformSource  | 平台类别         | 固定字符串”10“                                               |
| time            | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss                                      |
| username        | 登录账户名       |                                                              |
| version         | 程序版本号       |                                                              |
| cdn             | （暂不明确）     | 固定字符串”1“                                                |
| videoType       | 视频类型         | 固定字符串”1“                                                |

返回数据包含如下字段：

| 字段名 | 说明               | 备注                                                         |
| ------ | ------------------ | ------------------------------------------------------------ |
| code   | 状态指示           | ”1“：表示获取成功<br />”0“：表示获取失败                     |
| ret    | 返回的班次明细信息 | 使用电子密码本 (ECB) 模式解密，可得到三个字段<br />"chapterlist"：章节列表<br />"updateTime"：更新时间<br />"courseware"：讲次列表 |

JSON格式的返回信息如下：

```json
{
    "ret": "2XmB6...",//省略密文
    "code": 1
}
```

ret字段解密后，JSON格式的返回信息如下：：

```json
{
    "chapterlist": [
        {
            "order": "100",
            "outchapterID": "0",
            "chaptertname": "第一部分　前　言",
            "chapterid": "127026"
        }
    ],
    "updateTime": "2021-04-26 14:24:14",
    "courseware": [
        {
            "videoname": "第01讲　其他相关知识介绍",
            "pointname": "",
            "demotype": "0",
            "pointid": "",
            "videoOrder": "21",
            "videozipurl": "http://downcdn.chnedu.com/downmf/hd2021/zhukuai/lijichu/kj_lgf/phone/acc131541a0301_v.zip",
            "videoHDzipurl": "http://downcdn.chnedu.com/downmf/hd2021/zhukuai/lijichu/kj_lgf/Ipad/acc131541a0301_v.zip",
            "chapterid": "127125",
            "audiourl": "/sec.chnedu.com/PT2GJzzBYHhZsV-d7FHGtXUNt.lTCMvs018lJU.gz.Z3fWnRifBbVMyc6aPZGfaVE-H0Qqhajdw_.mp4",
            "videourl": "/sec.chnedu.com/PT2GJzzBYHhZsV-d7FHGtXUNt.lTCMvs018lJU.gz.buDuVXLiSl3cyc6aPZGfaVE-H0Qqhajdw_.mp4",
            "modTime": "",
            "title": "第01讲　其他相关知识介绍",
            "audiozipurl": "http://downcdn.chnedu.com/downmf/hd2021/zhukuai/lijichu/kj_lgf/phone/acc131541a0301_a.zip",
            "NodeID": "0301",
            "videoHDurl": "/sec.chnedu.com/PT2GJzzBYHhZsV-d7FHGtXUNt.lTCMvs018lJU.gz.buDuVXLiSl3cyc6aPZGfaVE-H0Qqhajdw_.mp4",
            "length": "01:01:41",
            "videotype": "1"
        }
    ]
}
```

chapterlist字段包含如下字段：

| 属性名       | 说明         |
| ------------ | ------------ |
| order        | 排序号       |
| outchapterID | （暂未使用） |
| chaptertname | 章节名称     |
| chapterid    | 章节id       |

courseware字段包含如下字段：

| 属性名        | 说明                                                         |
| ------------- | ------------------------------------------------------------ |
| videoname     | 视频名称                                                     |
| pointname     | 知识点名称                                                   |
| demotype      | 试听                                                         |
| pointid       | 知识点id                                                     |
| videoOrder    | 视频排序号                                                   |
| videozipurl   | 视频zip地址（标清）                                          |
| videoHDzipurl | 视频zip地址（高清）                                          |
| chapterid     | 章节id                                                       |
| audiourl      | 音频地址                                                     |
| videourl      | 视频地址                                                     |
| modTime       | 视频更新时间                                                 |
| title         | 显示名称                                                     |
| audiozipurl   | 音频zip地址                                                  |
| NodeID        | 视频id                                                       |
| videoHDurl    | 视频地址（高清）                                             |
| length        | 视频长度                                                     |
| videotype     | 视频类型：<br />”1“：zip<br />”2“：媒体文件<br />”3“：仅讲义 |

#### 10404 产生视频密钥

通过该接口可以从服务器获取视频密钥，接口请求地址为：

```http
POST http://mportal.chinaacc.com/video/header/genkey
```

返回产生的密钥：

```none
FEE222F11080552AA434455F169437E3D22D120D
```

返回产生的密钥

#### 10405 获取视频密钥

通过该接口可以从服务器获取视频密钥，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/versionm/classroom/course/getCwareKey
```

需要传递以下参数：

| 参数           | 说明   | 备注 |
| -------------- | ------ | ---- |
| cwID           | 课件id |      |
| cwareKey       | （暂不明确） | 可选 |
| ltime          | token的令牌时间 |      |
| pkey           | md5校验码 | 通过以下字段计算得到：<br />cwID、<br />platformSource、<br />version、<br />time、<br />token值、<br />固定字符串"fJ3UjIFyTu"（用于加盐） |
| time           | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss |
| version        | 程序版本号 |      |

返回数据包含如下字段：

| 字段名   | 说明              |
| -------- | ----------------- |
| code     | （暂未使用）      |
| cwarekey | 视频密钥，长度为8 |

JSON格式的返回信息如下：

```json
{
    "code": "1",
    "cwareKey": "cb100925"
}
```

#### 10406 获取视频文件头

通过该接口可以从服务器获取视频密钥，接口请求地址为：

```http
POST http://mportal.chinaacc.com/video/header/GetHead
```

需要传递以下参数：

| 参数          | 说明                       | 备注                                                     |
| ------------- | -------------------------- | -------------------------------------------------------- |
| userName      | 登录账户名                 |                                                          |
| cwareid       | 课件id                     |                                                          |
| videoid       | 视频id                     |                                                          |
| videoType     | 视频类型                   |                                                          |
| hash          | 文件hash值                 |                                                          |
| priKey        | md5加密后的密钥key         | 通过以下字段计算得到：<br />hash、<br />生成的加密随机数 |
| time          | zip包里的视频时间戳        |                                                          |
| videoHeadhash | zip包里的md5加密后的hash值 |                                                          |

返回视频文件头：

```json
22D022F10F44BBBA9C62DFA772BCE8F43FDE3B864B58DBF854B4C289C1EB12ACDA529C602E47ADCEA3FEB5F25E3660093FEB0CE86F15CA028CEF2EF25A62C2519D7EB7E61AE1C0DAE795E6A37705C0549863C16D161F9AA9D5BFA2898C9E665D694A29D1F05924307DB26E8E11CC5A0058F6DCB32FBF8BAA230E2F1559DE32930E6A255CE37DEBBCC55F7E7E7ECC62CC1A550CFBE199B5736899EB34653D25120D116F11A4E1CD965FA2B48677FAB18816735FB3F852F1CC8796D7503D57B2BF8071A38630136E6BB20D725B62020634F1BF6FC33D552155F2B84027E862D8CF79323435975EBF1A42431A4FAA1C1F8A92096E9A8A830F5E3BF6307F9C3B1D1BFFF1FA8562E9D83B3C432007E67634C338B5CA3735CC506D3097EFA0A07F3A1B425790609F86087ED59E34103DCE613C79AC0A02D5DB7A199558719620AC4B8279A6AAD96A7F90B04FA6B6132709EC2E94D4557E1AC05069C387BAFF1CEF1AE1D058501918F3E75427AFC030D78265FF4F8B076281F3F357180DBD729B07BA5E2B8E4521B9745647243C5D5AF01D3F917E2497E435461027E3CDB29280F1C58160D525FF6252234585E0E10EDE1054483FB3F3CC95FBB3839DBFEAA86307E94B04CFFE2C0E358805776942D31BF734A3DDBD1EADD23858996CB17D833F42A5149C8F9AF082D7A839256D63A018B988F16C6819B9154A919E294549362321E030469EF7C17A2757D3750A3E429BE2925052136CDC7E60F4824778F86CF6990823B31B818E926B5DB046C3F6FB7EF3693B33C8E49D8A544FC9FEC5B36CBCE30E9D0C4F0BD9F55B49ACF3FEFF630C072FDBF069BEF7B8A7E5148471D010B700EF766C8F4BA2B4B6EEFE22B0220200B9C2613BC87A3B7975C1941AD330E37351F68A031F9C7A12BF1E10C8E29209F20AD8F8E1E15F95D83503D5BB75C699E3FDD76FA2E586B2705347DA484BC502FE4EC49292DC050DF03273DD7C7557C70B4F8F5672F6E6BC296054FF97336DEC2074AF9CC87F6E5C417AD09F6F4A294F0A4D663B69AFE53BD08F8557DD09A1B01F541001D7DAD1F19238EF9271C63037362619850F56EE4E44C223683C2CDA5946054C24AEA3087981A92EBD741475CC57A5D97A15568459F118AE4EEA9868CE7C54399460B041EA72116D624448F95096244CE40978FA2B126BC7E885471CAB83A85F45F607DF358E4DD3AC0C933933D11812EBD3C99868825C2A0BC861835624235ABBAC0B74A0DF2040530C56EF742E9C16F4C07771BE6F785D37D9D1A76DD93D2DC16A233F6D321C8885D441E38BB5B9E805467C7A76ABF6A0B13827756B231F15E025C9CAF51E08265AE8809F3FA19379A30C93DD37D5ECB4E231BE9854A4C6E2C41D7DA657170EEBA5E54737FFB0A49F4B403C3531C769882EAC070AD2EE451985ECD47C2D11CEDFF857D0E1393FC5CAC5F09ED9B47B118900DE1D6CDCC23376ECF29AC10B473570139EF29B50208587A6E11FE952C3C7A93421D1F2E1C48639E2226B4D8487456F0611942F77D510527FE0930A95D9D3D5C59075EED0483038B0FE99DF7A9E8DE2B1E7E818B2B75B35E3CDAD254F71C4085CBB8F6E6EFE206FA988270528BA2CA44DB8B53B8E3A96FE24CD9760856244F9613E22448C3370E2694F8D66C5D29669DB735A16D88FB7595CD48FD7ADC67BEA0EC4086EEB1F401AE9637EA547600833B3F84BD02722299768B19DA3A71A73591FF9724E49B831996D08D85E63450D303202D9742EA6CF3C4CDCD505D7C5B5874103B8AD1D03161AD8F4625F3B9E9405446A6F9F2FC5942E5BF81DA39AA20D1EEF411EC7422A4C99F55BA41DFB92689949114A4B34BCAD8A2BF344B9B569DE4DDB15A4984DA844146F2781A20B117D9E81C81C8A4D38FF92AA5A09DBA1FB4EDDB495C500BCEB63F62D01FC862D5C6A6D3EA83ACABF28BD1A9748CD8E11F2F7EFD81379CA21F0C34B2A6978BB89FD22BCA02205AB1F24F84E71E4C3512A6D5491A9D37A5D7D0ECFB2E09FDAB2D55547FCCE58C85AF7D05317BC6385598FD07A546B83EEC80F6EB4B3EF56A8921B859FC3DFF7DDF78B8FA60F3121FCFFA0AEF2FB6E89C9649D57747DBDA4E8D33E62281E93F6C42E66E79D02B1C167974E9B7B555AD37CDACB9E385E912BD939E6934AAA143FDAFB70EE1A972723F28B802ACF82B44A8B4D41E6AAB3D6FEC6EFF13A3AD6F904C19C2642974F8FEBE58E21C1B248C895903BB67D151B06358AD5445DA904C0144E76CE682F53C5A0FD27F899E32AE874B9690728502895886DEE3072F5C9AD82CA5D15B706804B9D7536BE728EA9891C9BA6732B6DA5C732FC8426B2A8A6E310CF2DDF12B96A614CE0CA032C814B81A3739DF8A270EBFD6B8322059EC5B837F82DEABE672613C25EC81D652794DED547C6CCF7D32FC3A9A333913F00443CEF05578FC3AF496EBD99CD87B014C4DEB1DCF36E8AF049BEC5189906F5257BE05023EE278BC466D9CE6D26160D81D4A646089BAC5A6A21900B53B7A243D770AAA0D90689E816C1D46EF9D7AE076DF7A7C1297B16BF180729D731B54D3CDACF72134FC9FA3619DE66C52583C679FA5EB0EA7340CEAEED25A5D0720DECD4DE382E4A07CC8CE831E86E77067960038C0396102CC67023B0B4ACA4CAB2C83B0599598598A82168986A69015172D3953CF7BA27CBF83EEDF1ED9138BF5EC00F91967ADEEC364CF68660906D7AAA323E87F55AB99F27B15EA88310F763A3454B49A48812E9A1977FB1477170414FFFFBCC26C020533B323025EFB5A7A230417EF642C9A94E4FB31DFA2E5AE25E213AD2344EB5BDAE661A8DED2FC0A5767A0D307896D79ED267DEAC242DF3C22D224B1222373A9BEA7F44025C4BAB224CE7B30FB93B1ECC1318415330BAAB94F8582571832BD7AE1992FE1A4B7BD5175BA60B70A899213C9AE01EC5B0C5528D3E656F63A56B287EB65CCF1AE9B93763D69A8B97628CFA2F6420C997F822FE87FEB0EEF7BC6E82A2F6A06ED8FA14737318921ECF312367303D30CB9FE08357E4C4ECAB7293226FB0123A8E688EA1142A06A59AA6C5C78192E60B9326579BADF4AF7B1B572624261CB6F6F07E5BF65EC25EF9EA55743AA18EC5843431BCEA396CD739F9A5471CCF5856AA1E77B65A127BB9026BC4954E90B1C98BE01A281B340DA76E94B327C77B286B620C0DEECAA6B42DB7C90AF67437E037CDCE0A109EABBC5C7A674FE5576AF9AB5937BD1440CC8E62E5C822099E8526B0C4CAA7564FB6961D5BB1361D95E3DE93C3F8D982E62FD6CB3EF4B20FB45F29D59D3F652780A16CDEF4A904860589F34C2E2C7152FF975D634F39C5D2872475F8D54A17ADD66C87E58448040F64A659E4E4A9EE8F5D0793EE5C1CB5877E8F396071A91A11AAD8F8BAF56EAD8D89DBA87CDE5D5EBA25A24D53A49E23416DD0DDCAC8E34BF66B850F0DE97FC64445BA8A9E8379786F80E0A2B04DEDD08F7A93E3DFDAD6B28E72139586CE230460A7E0262044D5E3538A3D51A490DB70612A89A0AD79E7F0A56EA4A02F7446F9D12D1C69AFB524C8667EDF0994D31B0DF0C7114A7A8AC741FD7419FE4E2F8D476E951371A44E1D5F760C4FD657409B6FAFF2937F22299778B3C689BE09C7B9B2FE8744BD13E5DA87F89304A2279D13C3FF0757FBE88D768E27659C7270590FC662CE2769A009115353133DFD3FC4E006E6DD4C7406E6A9CCE32F6216E624090FB1D31856A315C9916F0016803DF364C754E61B3A677F129FC9563D0EC894B9B3C672BD3F54B1660FC3BFC99F6344A9305F7AD8A0C9CD1CC521CD5CC95A0221B48B2D90DD1BA8A9768B2E74155412D67D7ED17B952E434697882BADC670751A13AA4B229E3DF7B42A9752D5A4F20327D3DDC104712D08F9575935CE93C92C1DDE8A4FDB69B53BDB0C9AC8FE7917E788B1C84DD379942DE252F4910B5B4173ADE7A21B6ECCA9C19D922434CE5F195A7244F3C1F63CEDA147B268783B99729738B97FD4D300E0EF2F53614AFD46F481051B17BCB8D63F0AB76E07C2879F675B3B33FB71EEE5C8FF7B90BB2B9C6C17BA7D1CD6626AAB743E93950BAE69FA70F8F7ED831211023F231EF8AAD25C448687B289443800D8A9D869921C5BC2E1E54EA43C39101F0032DDE3A0EF679C48CA6B5C67433EE9F676672B21BE406837EDF92FDD5D8E15C328F287892C6B6AB435BDADD4957E5914FEB2C2AABF0C253EB2F2CEE8DE36D6E67EC8B5D3B0D42658832C79503000721FA5EA07C24CCBB8939970CFC79D886D8295C9D1840DEFC7D598C35BB72D84711548AD0AEAA87987066F90867C36938C4543B33BD1071A519208665C7A3711E552B498A35C6B2EB0A84FA2610660E33EE01FB373BB0188E5A377D1731AFB52A6BED018E166312E045E3DEC91B1BC624164B278AB78D9F9B0937762A48FD2D07574F474A5A3BBAC6466C8745D9DF6E91E4CD86CB17BB0B9A8A3D020720A28C9714D2094739528A437B58D6CD5DB5049E5B435D53AE1C0ABAF0252D140A3BF7E9C32F3D2834FB82D33FD802E7ED346D56B3BB316E398B970808082320121BD930116F9F84ED7B411650A0ECEC58A3FCE95D5B562B52BB1BBB7B29690B77DBFA256482C5C0B16EE791EF82CE9F17753D172719D5478CF30AE9C26CD55867141832429B969428C97999CE97271D09AC07BBFA241147A974D55F758B07762F7627A5E637651078225B1495DBB9ED105BC4C6B9D355F9ACCFDF17ED0E9B285BD1B6B2D5E9D985630384E3F4F78F53A733A7DDE5185CCD6CAA9973B8C6378F77E3D5CA1B63ECB75EA72ADE281FDA88F5ECA0916EFCF177FEBBCFC850FEDBF861DC73B2C775F3B3931C390DF83AB1E26425F0E96FC12C7E8CF2F9F85A5C4803B0AC7D0E5794492DFF648150596A836AE674A661C1DE10C089AE736F7DAD3DA17F4078E83550C788A5135F6B2F998FC3966CFEAA20E366CD5B8F5766FF1111472DF2BF504045F67E62D6585A259D7B1FC3D27584276A9DE9CDCC668F35A06AB86C84A7FF38D1905CFB15632FEB4A40684215181B19E2C137CD58CDD39833477A39054738CEA896B4DEDDA4D18984FE831C5BF61DCA43482A128AC3D0DC025AF05B4DBB3F78E9739136F349EEBBC433B0EECEC7CEED4FFE066F19CBDD3968B7D162F9AC34EBD1E1680D9B49FB19F318BC937ABAF3B1EE5B4F0251A57EE864292E6838B509AB550EEFAA7721789C416D202552DEE9276ACA0FC4D22DB487887C062E3582A77CAA0C697F9BFD745AADAAA794078B08103B6FC3F1D3976E20F42749810432446B4E61800BA64A038A84B76559D5196F55CF4AAB9EE61E2303ED689950D18F80066C50F39DB4D6BF1D4604851DE20BEE0C128F3C104FB4B2C03D7E990DBB766F603DDCF03D7DAFFD942ED562E210B309CDBF250CEA62BD223EAEC6947C08F1063C3E5EC9B1B709389F64A760348A978AAA66603C6192DA31E3510384D623953EA6050605E425612BFBEFD84FC6EB38FCE44B7FCF5556635BEFC9041E16AF5B85795022975892B445D3B1DC576D67CFA1ED2794B5B47A3596B3656BB535FF9289801D75E775D426FA6233D5F7F66605769F8E54E1895011F54574A178F022732E07AD499A397DAFC879B3C25BFB4A7BB2076EBB9A801283C09ED212CBB2BCFCF2314E31BC4132DEE72B6B3D28B48C5F50451F49E56A8191B25BCDD83AF27E52C8C2D6C58FBA5567856109EA2DBF63F7106C105D19D79CA8CE7A55E7AD9C3ED2A74A3BD164DF09A2B823412216305444135B1B6D2354AFEC21A9F688C01879AFAD8CC17542445C58471B81833CC48ACA4967D28737025787D4738
```

#### 10407 获取视频文件头（直接获取，不带密钥）

通过该接口可以从服务器获取视频密钥，接口请求地址为：

```http
POST http://mportal.chinaacc.com/video/header/GetHeader
```

需要传递以下参数：

| 参数          | 说明                       | 备注 |
| ------------- | -------------------------- | ---- |
| userName      | 登录账户名                 |      |
| cwareid       | 课件id                     |      |
| videoid       | 视频id                     |      |
| videoType     | 视频类型                   |      |
| hash          | 文件hash值                 |      |
| time          | zip包里的视频时间戳        |      |
| videoHeadhash | zip包里的md5加密后的hash值 |      |

返回视频文件头：

```json
DFE763C04CB296C0DB1E1E9FA37A91B4E0E90FAC71B651FBC198CF98B3B6EEA0FC85A49B02FFC918FC0FBAF501BE6B935037186C7C914C5F8D8251C39A657F2902088A222BDE33769E67EE6F79ED5C08DF04009C69B11D8137B46321EB956EA94D1B88DA40C6F47BB8A435B7495B09EDCA1C4A576CD02E9485A639528E3E3A06218A2FB77B7A87D09A45CDCCE0591180D8C80ABE3BA5827455245B1DEF0607F4CF6C1310C035A893ADC421A489842F79B4A4B0F3FD68AC7F2DA68607C921F00783285302E13BEE03D57FC3B4A1BE7DB0AB6CA6AA447BF917E1127CFF8721BB4A85BDD4FBE50FF0D5275790C2B9568DD93B1EDDD089D051ABE99CBAC2A50EC8899869534A7E4E290923020B51BC54854D746E9DD74C6D46BCE551F3D9DE0B3F479EBEBDC6B18730836E6A89F230BD10D246D32098F5AE8479DE06997BC0827BEC499B5CF5029B9FCAC22FAD03E57DF404C15702862C223C55F79267C47965C185DD4AC9FB3CE3426DBF652A8A7E1F9CE9A26CA3387C2E666273D7079A9136BA9836617D79EEF6E40C510AF0CFFC90BB0F655B1450787486CE58215C6F982316E44653D428937407E08FB8490B150F941EECD1AC7C46DAF9A21CC6813B4E810DD28338D57C9C810028F620DA4B44A1518BBB90D7D2496769A596701C238603D61E80C220E03E6CE349592F67EF694A2343E4EED9444BA6F48BFE48E5B573368A0A0C8A81D60FFD07542E4E59352582CD4CED4BB4E8F65CA26BF2EA062AE348C29C8A7FFFB8F7025B7EE68BA7DAC7ADF13CFF24C49149573D50948368BB7E5434D0927097E3CB74E7787A148E1797CA424CAC992B4684B0857FDA796728AF1411A3630565464C9B0FE64225327C4E15AA0ED797960F37037F3C0BEA5686173A9F9F8C958BD713B7AFA8565AB1AAA16CA4ED2739F343BCFBB39B62817AF27F87B27F7C0C85241E80C1B6DB90EAF03BB164B4B1A12040C1ED7EB15881741E9491E5524E913221C67FB7BDBD119FC6DA8C7E916797FC9242C4DCCE04A0A5D9714DF53601C256F0F3CC6209772F6E8B64AB700C654F299B3B452D25C012AD474DAD140159E732A1061CE18576A4C58C858BE010E9E730D8E68A49968B867F3461E3EC897E7D21A6D702B81BC9CA96B310350C9061B3F0E385CDE06E344022E7065AADE85E84A0E1BB9051D6126981B91283ACD32D11C8D4E0E4289374E7305484ECCE7DA4AEDEC3BE16DF8EFB310F1BF7A9EFAE71045A2FB6AA2C58ED9BE289DB6071A6238588C9479651E160D6CE36505897AF0295D21578E2D8EE3A146C4B9C020B5B5BCD87EF0AB09D9839AFDD375B6C21BBD6FEB01448597ECDCFCD92E8468700341AAA439D50DB9A1D40719AF1E7E42B7A903C76B30F29AA0AC76F681461033418E9920F30A1FC9482B4D6CE7FDCB98C47B7F4C442D9F91973B30C397A119D97CA6E467E5510ED66A60890C06E7E5C2BD53FC729A1F5383F7EA505572E4749FF8929FF9D9BCCE2B67B53A085EBE6346EB01E29E45218DDD16B22ECBE8F265885E849FD53987A310CA91AE0D519878FEED17274F7FFADAC213B64E4B6F15AD3B0F1137F32C99F2EC0592BBEEC600C53EFF2C37EA4315647729E1F17B426E284F33077DC896FFAE45BDE146683296E09EA63BB80ED2283DFF002ED781908870E07CF67FE9287B35E0EA10F418671C9B1BC442574AE11DA6C307DD2DDDFCFBD405610F59DC3A34BC5D07F3CDBB437F8EA3BFD4761FEE36282A41B44A65D900AE220FD9FFEFBAE733BCA21992B2A6C6ECB017D845D7E7517E0AD1947AE8E5FBCAAC2B8E1C5A978E104631421076B5F1235BFC95F4BACC8CE6E76704D85BC7A71D2298DD6E807D8F6D94EAFF89B05E5AE0BC197C0A55A3AACEECD4617C50DB188185F9839A78227D4A9023794C8286C849C9C52F3254996D4E9DEA4DAB50498A6C01ACE093833C33AA76750C889E381A00A2D251C67EAEC323BFC058E47841862F6F24A52C7E6714B975C46E87848E8B8DA8DEB7D8FE6002A42A6FEFBB4BD5F0AA3B91EB5C114B8DCD038AC3DD8B426A27BF41699300CF8D63E714BF2206D8D5BC4B0C460C7C3BF775920CE3D61927ED2F1D85907673BE7B29548969F988FABAC2938E6BCD6C0055987FD663C158B6E1606CF434683AAAF8483F62F3DEDD23A26739EF06A200405FDCA746BA988422B75E33492A4A663D1403222EBE746BC2683E3E7A2E353689EE4F2ACDCBF3C900DB441F6F60CAC858C98765F3C0B305876844F2D82B5DB494F93E6087C1BDD94CA160C830829A4D42A5E838611DA2F731728FD001F2D75D41D93572CA8D469507BA870F2F526429FC888321BCB8EF01D9EAF7D560F8E1B9EF64C1E77AD38D092E2E56128E8B83A69261FC8FECCEBB0D1A3011E7547A5C2A67F90690A6091F0FD6D776C3634B6DEB2DAEF9EC1F4F7EC0D65D7BEBFF7029F90D838C943709B7D14A9388372F7DEF7BC6C7334413D18E2DA75C7460BCDA60F6EE74147D7C1A6A28FF081E977C65F1B9BF9D53C59BCD5D3530A18AB14FFF4D9DCF786B0B4D7CEA869E5EEF105192284FF95B781B477C5CDDE683EB55D67C65847E4923518A0E903A03B35419686B670D4FA556A6E681F9068D61418E276BFD1A23FD32443F795AAEF66650B1DEE05EEAAEDBDE5DFE5357F2CBA1B39E2776D4B4B8B0791409C6B9E55BB1937B193DF905026587A80468261F7AA7FB6318033687855445A973200DBCD62430ED2121C20A8EAB79CA0703BDD220DFE4CDC184525B4A7C9EBC14A4361DCD6CCC639050212F0F9EFF6C342284508895C17C5BD619B432D5BFB33D5C57603878374A293695266CABA7102766F5A9B675554855E4250F069A3BDBA61774F8CFB8209A8BD8BE68D8BEF75340CD066BE6771C3B41F4EF8311649EF38A28AB4DFD726C9FDF72725A17A8B9AD5EEA7A8DD6A95575696D2BA1B33576C9685A2D7E6648A0B6117D7C7266AF7DFD89BD158444348C56C58B291E983106ECE0B6E9FF5C2E4787FF4A3A827051A54ED6247C4AF15C1F25051258772C64AE4272CE671E855D4BAF0049826E90AFA9B0AAE55974DD1857094793C3BA8D132371766E5E999EB01A71F5830207FACAE257DA159232BC72BB1E91458845E423B7B230D4569910C374E9FCFCC8C94B7A22908B1715B8B5A3867678454B92F1431C0A588A959D726FDA821539BB30BFFEEECBB4717730B20D5706BAC1757376F68FF8FE81642ADF73ACD71BA7BD3C179ABD33F8E050B46D9AA208481E66027C10670B98BE461C6A7E140B33AB665DD97055356E448777229B264DF03BE6DA6B915BF42787DF18726034808EB210DB280CC33235F6BE4EE89AED551D8E45BE8406DEB996B3A0D110BA78AE20D2B38FEE541247AE4A8C5031F6BF208230E8CE613690C9A92B512B90E15A39CB885067DB7CBDC225091D598838011843B4A9D58CEAA3810E177BB31F67C990BD5B5757322FF38D58FD5AD3BA32D1D5EB11B29631D21182214044E9E7125757D59CE15F104E57D155903AA2877C63BF9A10B000DC37F0C8F973F92F66C5982547E1DB52BA37D9346188173A39FA9B1BB775B5726BB99046EB7421EEA960D353EA0FB0A0B4F652E95644DDDBFA431266E867EABAA6C4DF1259B0E4DDBE5358514AFA7050B7A39B158A6233063C98E5F81AD16A9487B9895DB3654A8D1C2BB90887FB65BB0D3A2C17148EAAEBB476EEED74FD39F4655F9396358062248EECBC201E8B57759BDBC84ED5A846D32D0247E310E336F1B83AB4064754DB13A202100BA64C7CECE151485F1B97EB28C956BF47780CCFAB3246E45D79AA972A0E30348FFB1DE7CE039764956C92188114595B5CF8F11490E47A6F15BE8D844AC2C0A90D1CE74F62F75800320863B962876E31750EC7470E773BE1C897DB6F2FD64E4D69CB82871402EED4723F418BF3B45166D5F2B6CEB6E36F149167C01F458D2BC2809BF30ABFA4343E31E758ED049A26DB920CD788DB57278126C910595DBE980BCDDE4A978908EC21C23FDC37359B385863D9C2F6D16F42646A5D6DF67E0E7266B3CB8C777BFAE101B48CD542C5414D446A993836DBABE2BB050A23E17FD361ABF03EEA57F23EDC16C9062575368D63AF621A3995ED4024199914645C3336DE0A00F30555065EBC5162EF01278307A37EEA14FCEE995521DFA8B9771679507138CEE79B2CBD8A82BDAB5D758D01700C18CA86C3A3B6E90108F62A823915DE2AF656ABE36E384D9425A07C257FBF5EBE9130C17599BA39593D35D3637E38CAC71FE48841F7191534C92BC166C4EE72C27671107F85914906A2EA90EE1F58FFFDC1EF60CBC77D0038CE34A09FCFDA7AD6353B3E2528ACC522D8BF2750EF272322AD60ABB32E228A398B3281C85837B9767F6855F4397515956EB88BC8E0257CF8ABFF9FCC03475FA9EB2A6B1DD4C62DB81C38FA42C39A1A9BC128885EF0872E3CDFADD617EC350CDB8D1DFF7F14016CAA72DD4236765F44F8576A2CE9039EC5A5DE068704DF87360CBDA7FF207252D0577353D0FE2D630464E4F74818869EABDBC5BCB06E6B70C0A3DCDBB8B45925C8D1BD6E74FF996D60C2A3DB4B84EB53BCB37FD51B311A8ECE993D21E66727099B1AAAE027102E52B7C7D952A44C8AB1D4C1C6330F4FACE6B1EB7847623E115685192D2789C849CD0F4E0CA9BB983A62D03A072D9D8CA30F96C1658D6258F1B246B79E32D7F95BB75F614CB20DB7EF433CD52E8C9E4439F5DE02AFAE98CF752E94916BCC05BFDE105A1362345370C971804D2A9EAC2B5307370DAEAAFE9D7C17C8A7C9F8E12F233FCFCF3D37F6F7189CE315B5E87050CADF82D700391AE80E78649F122A4C690554C730F392D466E82B7402E573F60BA6825F90542C757EDF303E8EE0150099F6ACC94C3C0FBFF2EA0375BD8C6F159C1526CCF5103658F10447A56350E923F2D4E97C621A09746B8A188B5933B0F6400258EDA06C1724F5E638E490927024D7C6760B4966DB1F8F83DDDC49008A8BEDE33536C7D1E03ECC8799992262A3168B849FB88BE96E6008C7C23B33F168BCF53D52204343DA3AF3288D85C6622CCA4B4ED9EB17071BCF1FC03D328731ECD54DA90188C20C2FFCF73CD3D17656C41F7A7D650C6B6C151E7E70A597E1EDFCF48CB04F42452DAB26A12A43A4B2024ACE22D447C6A8CDD528F478CA2CDE24FF3C89E8640B51CC77B6817F1FC988E79EFD7BE1B412FD4D08C37E7ED2E7DB5C4F9DAB6011530780003D231666780B47218606F4D379156F5709414B01CC09178427493C4FE8263A34146FB4BD1BA6E51D51D2903FAEDA68C76A90CAD4D9704CA8C9364CA254C50462DDA8202D94F1023BC3F0BD5C7027ADB360109A65D614A9673F44485FF65DE530BA539E2CBBF6BE275066FEBFD844788801DF6758F078BF2DCDE829A6E377CCA95F033FE8353C113D82B78F0565DF0D3812FBD6C3DAC63A52F7B4317277F640837B5EEE6A99600680169BC13116A36D051786F3E48EED2452EFE83E179507887B89329F283F16CB20D499E383CC57B7E6C4B70EFCD9FFAD4F11C74092C4A9389F2471F03AEE95A1D6B04172A893D1BD7A1C6F6589B754D2029658954ABDFBFA49A923FD91C0A8532BEE3A000EEE781831EC79B3A34180BE42996239EE728DF6880FBF1BDD9B182EFE3800A0863D8F2B91066300179A02ECDDCBE8880344A7AA5A292F62C826DBBFF79206375DBBB1E4F1165AA0D75C4BF2E569D7F322B1FF8848BFCAA214054B094F9BFDC6D1C9BDEFCEE4E65B0950FC0D0B7E4799DA911C7DE6CD63FCDF5294A4068A2010FB9C0CBF4072497CD4A55F3E5326FF5B650C04C43CEBBA07D7E0633F49195
```

#### 10408 获取视频对应知识点测试及弹出时间

通过该接口可以获取视频所对应的知识点以及弹出的时间，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/classroom/versionm/cware/getPointTestStartTime
```

需要传递以下参数：

| 参数           | 说明   | 备注 |
| -------------- | ------ | ---- |
| cwareID        | 课件id |      |
| ltime          | token的令牌时间 |      |
| pkey           | md5校验码 | 通过以下字段计算得到：<br />SsoUid、<br />platformSource、<br />version、<br />time、<br />type、<br />guid、<br />固定字符串"fJ3UjIFyTu"（用于加盐）<br />token值 |
| platformSource | 平台类别 | 固定字符串”10“ |
| time           | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss |
| version        | 程序版本号 |      |
| videoID        | 视频id |      |
| isMobileClass  | 是否为移动课堂（移动课堂功能已移除） | “1”：移动班<br />“2”：下载课堂 |

返回数据包含如下字段：

| 字段名            | 说明                   | 备注 |
| ----------------- | ---------------------- | ---- |
| code              | （暂未使用）           |      |
| pointTestTimeList | 知识点测试以及弹出时间 |      |

pointTestTimeList字段的列表项包含如下字段：

| 属性名             | 说明           | 备注     |
| ------------------ | -------------- | -------- |
| backTime           |                |          |
| pointName          | 知识点名称     |          |
| testID             | 知识点id       |          |
| pointTestStartTime | 知识点弹出时间 | 单位：秒 |
| rowNum             |                |          |
| pointOpenType      | 知识点类型     |          |
| cwareConfig        |                |          |

JSON格式的返回信息如下：

```json
{
    "code": 1,
    "pointTestTimeList": [
        {
            "backTime": 1,
            "pointName": "在某一时点履行履约义务确认收入（综合）",
            "testID": 29044369,
            "pointTestStartTime": 2044,
            "rowNum": 1,
            "pointOpenType": "t",
            "cwareConfig": "10000000000000010110010000000000"
        }
    ]
}
```

#### 10409 获取知识点测试的题目信息

通过该接口可以获取知识点的题目信息，接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/classroom/versionm/record/getQuestionByPointTestID
```

需要传递以下参数：

| 参数           | 说明   | 备注 |
| -------------- | ------ | ---- |
| ltime          | token的令牌时间 |      |
| pkey           | md5校验码 | 通过以下字段计算得到：<br />testID、<br />pointOpenType、<br />platformSource、<br />version、<br />time<br />固定字符串"fJ3UjIFyTu"（用于加盐）<br />token值 |
| platformSource | 平台类别 | 固定字符串”10“ |
| pointOpenType  | 知识点类型 |      |
| testID         | 知识点id |      |
| time           | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss |
| version        | 程序版本号 |      |

返回数据包含如下字段：

| 字段名       | 说明     | 备注 |
| ------------ | -------- | ---- |
| code         | 状态指示 |      |
| isNewQZ      |          |      |
| questionList |          |      |

questionList字段的列表项包含如下字段：

| 属性名           | 说明               | 备注                                                         |
| ---------------- | ------------------ | ------------------------------------------------------------ |
| analysis         | （暂未使用）       | 可选                                                         |
| answerRightTotal | 正确答案个数       | 单选1，多选有多个                                            |
| answerTotal      |                    |                                                              |
| content          | 题目内容           |                                                              |
| optionList       | 选项列表           |                                                              |
| parentID         |                    |                                                              |
| pointID          |                    |                                                              |
| pointTestID      | 知识点id           |                                                              |
| questionID       | 题目id             |                                                              |
| quesType         | 题目类型           | “1”：单项选择题<br />“2”：多项选择题<br />“3”：判断题<br />“4“：其它题型 |
| quesTypeID       |                    |                                                              |
| quesViewType     |                    |                                                              |
| rightAnswer      | 正确答案的选项标题 |                                                              |
| score            | 题目分数           |                                                              |
| splitScore       |                    |                                                              |

optionList字段的列表项包含如下字段：

| 属性名     | 说明     | 备注 |
| ---------- | -------- | ---- |
| quesOption | 选项内容 |      |
| questionID | 题目id   |      |
| quesValue  | 选项标题 |      |
| sequence   | 选项序号 |      |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "code": 1,
    "questionsList": [
        {
            "quesTypeID": 3,
            "pointTestID": "29044369",
            "questionID": 20099280,
            "score": 1,
            "answerTotal": 1,
            "analysis": "",
            "splitScore": 0,
            "pointID": 20098990,
            "content": "如果与商品所有权有关的任何损失均不需要企业承担，与商品所有权有关的任何经济利益也不归企业所有，就意味着商品所有权上的主要风险和报酬转移给了客户。（　）",
            "parentID": 0,
            "quesViewType": 3,
            "answerRightTotal": 1,
            "rightAnswer": "Y",
            "optionList": [
                {
                    "questionID": 20099280,
                    "quesOption": "对",
                    "quesValue": "Y",
                    "sequence": 0
                }
            ],
            "quesType": 3
        }
    ]
}
```

#### 10410 上传知识点测试的做题结果

通过该接口可以上传知识点测试题目的做题结果，接口请求地址为：

```http
POST http://member.chinaacc.com/mapi/classroom/versionm/record/savePointTestQzResult
```

需要传递以下参数：

| 参数          | 说明             | 备注                                                         |
| ------------- | ---------------- | ------------------------------------------------------------ |
| cwID          | 课程id           |                                                              |
| ltime         | token的令牌时间  |                                                              |
| pkey          | md5校验码        | 通过以下字段计算得到：<br />SsoUid、<br />testID、<br />platformSource、<br />version、<br />time、<br />固定字符串"fJ3UjIFyTu"（用于加盐）<br />token值 |
| pointOpenType | 知识点类型       |                                                              |
| questionsInfo | 题目结果信息     |                                                              |
| siteCourseID  |                  |                                                              |
| testID        | 知识点id         |                                                              |
| time          | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss                                      |
| userID        | SsoUid           |                                                              |
| version       | 程序版本号       |                                                              |

questionsInfo字段包含如下字段：

| 属性名    | 说明     | 备注 |
| --------- | -------- | ---- |
| questions | 题目合集 |      |

questions字段包含如下字段：

| 属性名     | 说明         | 备注 |
| ---------- | ------------ | ---- |
| questionID | 题目id       |      |
| userAnswer | 用户答题结果 |      |

返回数据包含如下字段：

| 字段名  | 说明         | 备注 |
| ------- | ------------ | ---- |
| isNewQZ | （暂未使用） |      |
| code    | （暂未使用） |      |
| msg     | 返回的消息   |      |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "code": 1,
    "msg": "保存成功"
}
```

#### 10411 同步视频观看记录

通过该接口可以同步视频观看记录，保存上一次观看的位置用于下次打开，接口请求地址为：

```http
POST http://member.chinaacc.com/mapi/versionm/classroom/course/saveNextBeginTime
```

需要传递以下参数：

| 参数    | 说明             | 备注                                                         |
| ------- | ---------------- | ------------------------------------------------------------ |
| history | 观看历史         |                                                              |
| ltime   | token的令牌时间  |                                                              |
| pkey    | md5校验码        | 通过以下字段计算得到：<br />history、<br />platformSource、<br />version、<br />time、<br />token值、<br />固定字符串"fJ3UjIFyTu"（用于加盐） |
| time    | 带格式的当前时间 | 格式yyyy-mm-dd+hh:mm:ss                                      |
| version | 程序版本号       |                                                              |

history字段的列表项包含如下字段：

| 属性名         | 说明             | 备注 |
| -------------- | ---------------- | ---- |
| cwareUrl       | 视频地址         |      |
| cwareid        | 课件id           |      |
| nextBegineTime | 最后观看到的时间 |      |
| uid            | 用户id           |      |
| updateTime     | 更新时间         |      |
| videoid        | 视频id           |      |

返回数据包含如下字段：

| 字段名 | 说明         | 备注 |
| ------ | ------------ | ---- |
| code   | （暂未使用） |      |
| msg    | 返回的信息   |      |

JSON格式的返回信息如下：

```json
{
    "code": 1,
    "msg": "成功"
}
```

### 课程讲义

#### 10501 通过购买课程获取课件信息

通过该接口可以获取相应购买课程的课件信息列表，包括班次及将讲义下载地址。接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID
```

需要传递以下参数：

| 参数         | 说明                                                      | 备注 |
| ------------ | --------------------------------------------------------- | ---- |
| eduSubjectID | 购买课程ID                                                | 必选 |
| ltime        | 访问令牌时间                                              | 必选 |
| pkey         | 通过sessionId、eduSubjectId、platformSource加密而来的密钥 | 必选 |
| sid          | 当前用户唯一标识码                                        | 必选 |
| time         | 当前时间，格式为yyyy-MM-dd HH:mm:ss                       | 必选 |
| uid          | SsoUid                                                    | 必选 |
| version      | 客户端版本号                                              | 必选 |
| vflag        |                                                           | 可选 |

返回数据包含如下字段：

| 字段名       | 说明                               |
| ------------ | ---------------------------------- |
| userId       | 用户ID，未用到                     |
| imsg         | 获取成功或失败，未用到             |
| edusubjectId | 购买课程ID，未用到                 |
| code         | 结果代码，0为失败，1为成功，未用到 |
| cwList       | StudentCwareItem（课件）集合       |
| videoList    | CwareProgress集合，未用到          |

StudentCwareItem包含如下字段：

| 字段名            | 说明                                 |
| ----------------- | ------------------------------------ |
| cwareUrl          | 课件地址                             |
| boardId           | 答疑版ID                             |
| cwareImg          | 课件图片                             |
| updateTime        | 更新时间                             |
| cwareName         | 课件名称                             |
| dateEnd           | 关课时间                             |
| classOrder        | 班次排序                             |
| cwareClassName    | 班次名称                             |
| teacherName       | 老师名称                             |
| MobileCourseOpen  |                                      |
| videoType         | 课件类型                             |
| cYearName         | 课程年份                             |
| rownum            |                                      |
| cwareID           |                                      |
| cwID              | 课件ID                               |
| cwareTitle        | 课件标题                             |
| cwareClassID      | 课件班次ID                           |
| download          | 是否可下载                           |
| useFul            |                                      |
| specialFlag       | 未用到                               |
| courseOpenExplain | 课程开通描述，未用到                 |
| eduOrder          | 未用到                               |
| homeShowYear      | 是否显示年份（1显示0不显示），未用到 |
| isFree            | 是否免费，未用到                     |
| isMobileClass     | 未用到，未用到                       |
| mobileTitle       | 移动端标题，未用到                   |
| openTime          | 开通时间，未用到                     |
| prompt            | 课件提示，未用到                     |
| wareOrder         | 课件排序，未用到                     |
| yearOrder         | 年份排序，未用到                     |

CwareProgress包含如下字段：

| 字段         | 说明   |
| ------------ | ------ |
| cwid         | 课件ID |
| thisProgress |        |

JSON格式的返回信息如下：

```json
{
    "userID": "33049216",
    "cwList": [
        {
            "isMobileClass": 0,
            "boardID": 0,
            "datNum": 0,
            "mobileTitle": "先学为快",
            "cwareName": "【先学为快】中级会计实务-高志谦（2021年）-特色畅学班",
            "dateEnd": "2021-09-14 00:00:00",
            "openTime": "2020-09-15 19:39:00",
            "classOrder": 575,
            "cwareClassName": "先学为快",
            "teacherName": "高志谦",
            "cYearName": "2021年",
            "rownum": 1,
            "projectID": 0,
            "cwID": "acc1315190",
            "highClass": "0",
            "specialFlag": 1,
            "cwareClassID": 572,
            "eduOrder": 123,
            "homeShowYear": 1,
            "cwareUrl": "http://elearning.chinaacc.com/xcware/video/videoList/videoList.shtm?cwareID=36834",
            "cwareImg": "https://img.cdeledu.com/CWARE/2020/1029/dde622cf74dbf8cf-0.png",
            "queNum": 0,
            "courseOpenExplain": "您所报的课程费用已交纳，课程将于新教材发布前陆续开通，敬请关注！",
            "updateTime": "2020-12-11 14:36:00",
            "wareOrder": 12475,
            "classOrder1": 575,
            "mobileCourseOpen": 1,
            "videoType": 1,
            "cwareID": 36834,
            "yearOrder": 88,
            "isFree": 0,
            "cwareTitle": "先学为快",
            "downLoad": "Y",
            "prompt": "新教材发布前",
            "useFul": 1
        }
    ],
    "imsg": "成功",
    "code": 1,
    "edusubjectID": "3099",
    "msg": "成功",
    "videoList": [
        {
            "thisProgress": 100.0,
            "cwid": "acc1315190"
        }
    ]
}
```

#### 10502 获取视频加密密钥

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/versionm/classroom/course/getCwareKey
```

需要传递以下参数：

| 字段     | 说明                                                   | 备注 |
| -------- | ------------------------------------------------------ | ---- |
| cwID     | 课件ID                                                 | 必选 |
| cwareKey | 加密密钥                                               | 可选 |
| pkey     | 通过cwID、platformSource、version、token加密而来的密钥 | 必选 |
| time     | 当前时间，格式为yyyy-MM-dd HH:mm:ss                    | 必选 |
| version  | 客户端版本号                                           | 必选 |

返回数据包含如下字段：

| 字段     | 说明                               |
| -------- | ---------------------------------- |
| code     | 结果代码，0为失败，1为成功，未用到 |
| msg      | 未用到                             |
| cwareKey | 视频加密密钥                       |

JSON格式的返回信息如下：

```JSON
{
    "code": "1",
    "cwareKey": "c1d5e24e"
}
```

#### 10503 获取课程讲义下载接口

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl
```

需要传递以下参数：

| 字段    | 说明                                                         | 备注 |
| ------- | ------------------------------------------------------------ | ---- |
| cwID    | 讲义ID                                                       | 必选 |
| ltime   | 访问令牌时间                                                 | 必选 |
| pkey    | 通过SsoUid、cwID、PlatformSource、version、time、token、salt加密而来的密钥 | 必选 |
| time    | 当前时间，格式为yyyy-MM-dd HH:mm:ss                          | 必选 |
| userID  | 用户ID                                                       | 必选 |
| version | 客户端版本号                                                 | 必选 |

返回数据包含如下字段：

| 字段           | 说明                                    |
| -------------- | --------------------------------------- |
| code           | 结果代码，0为失败，1为成功              |
| msg            | 结果消息，未用到                        |
| cwareClassList | StudentWareKcjyDown（课程讲义下载）集合 |

StudentWareKcjyDown包含如下字段：

| 字段          | 说明         |
| ------------- | ------------ |
| samllListName | 名称         |
| cwID          | 讲义ID       |
| jiangIiFile   | 讲义下载地址 |
| smallOrder    | 排序         |
| smallListId   |              |

JSON格式的返回信息如下：

```json
{
    "cwareClassList": [
        {
            "smallListName": "专题一　会计基础知识理论",
            "cwID": "acc1315190  ",
            "jiangIiFile": "/zhongji/2021/yuxi/kjsw_gzq/kjsw_ljcyx_gzq_jy0101.doc",
            "smallOrder": 100,
            "smallListID": 124751
        }
    ],
    "code": 1,
    "msg": "成功"
}
```

### 我的题库

#### 10601 根据科目ID获取考试中心

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID
```

需要传递以下参数：

| 字段         | 说明                                                         | 备注 |
| ------------ | ------------------------------------------------------------ | ---- |
| eduSubjectID | 科目ID                                                       | 必选 |
| ltime        | 访问令牌时间                                                 | 必选 |
| pkey         | 通过eduSubjectID、SsoUid、PlatformSource、Version、Salt、token加密而来的密钥 | 必选 |
| time         | 当前时间，格式为yyyy-MM-dd HH:mm:ss                          | 必选 |
| userID       | 用户ID                                                       | 必选 |

返回数据包含如下字段：

| 字段       | 说明                               |
| ---------- | ---------------------------------- |
| isNewQZ    | 未使用                             |
| code       | 结果代码，1为成功                  |
| centerList | StudentPaperCenter（考试中心）集合 |

StudentPaperCenter包含如下字段：

| 字段         | 说明         |
| ------------ | ------------ |
| centerParam  |              |
| siteCourseID |              |
| sequence     |              |
| description  | 描述         |
| centerName   | 名称         |
| providerUser | 未用到       |
| openStatus   |              |
| centerYear   | 考试中心年份 |
| centerID     | 考试中心ID   |
| centerType   | 考试中心类型 |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "code": 1,
    "centerList": [
        {
            "siteCourseID": 272,
            "centerParam": "",
            "sequence": 1,
            "description": "高效、特色",
            "centerName": "2021年预习阶段练习",
            "provideUser": "",
            "openStatus": 1,
            "centerID": 28997635,
            "CenterYear": 2021,
            "centerType": 3
        }
    ]
}
```

#### 10602 根据考试中心ID获取试卷信息

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/classroom/versionm/center/getNewCenterPapers
```

需要传递以下参数：

| 字段           | 说明                                                         | 备注             |
| -------------- | ------------------------------------------------------------ | ---------------- |
| centerID       | 考试中心ID                                                   | 必选             |
| ltime          | Token时间戳                                                  | 必选             |
| pkey           | centerId、platformSource、version、time、salt、token加密而来的密钥 | 必选             |
| platformSource |                                                              | 必选，默认值为10 |
| time           | 当前时间，格式为yyyy-MM-dd HH:mm:ss                          | 必选             |
| version        | 客户端版本号                                                 | 必选             |

返回数据包含如下字段：

| 字段            | 说明                         |
| --------------- | ---------------------------- |
| isNewQZ         | 未使用                       |
| code            | 结果代码，0为失败，1为成功   |
| centerPaperList | StudentPaper（试卷信息）集合 |

StudentPaper（试卷信息）包含如下字段：

| 字段             | 时间                       |
| ---------------- | -------------------------- |
| ceateTime        | 创建时间戳                 |
| totalScore       | 试卷总分                   |
| paperName        | 试卷名称                   |
| courseID         | 课程ID                     |
| paperID          | 试卷ID                     |
| status           | 0否，1是                   |
| isContest        | 是否竞赛，0否，1是，未用到 |
| paperOpenStatus  | 未用到                     |
| paperCatID       |                            |
| contestTimes     | 试卷限制提交次数，未用到   |
| paperYear        | 试卷年份                   |
| creator          | 创建人                     |
| paperVIewID      | 未用到                     |
| chapter          | 未用到                     |
| contestTimeLimit | 做题建议时间值，未用到     |
| isFree           | 未用到                     |
| chapterID        | 章节ID                     |
| sequence         | 未用到                     |
| suitNum          |                            |
| paperViewName    | 未用到                     |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "centerPaperList": [
        {
            "createTime": "1604541345000",
            "totalScore": 2021,
            "paperName": "第一部分 会计基础知识理论",
            "courseID": 0,
            "paperID": 28995053,
            "status": 1,
            "isContest": 0,
            "paperOpenStatus": 0,
            "paperCatID": 14,
            "contestTimes": 0,
            "paperYear": 2021,
            "creator": "bianjunying",
            "paperViewID": 28995054,
            "chapter": "",
            "contestTimeLimit": 65,
            "isFree": 1,
            "chapterID": 28968808,
            "sequence": 1,
            "suitNum": 0,
            "paperViewName": "第一部分 会计基础知识理论"
        }
    ],
    "code": 1
}
```

#### 10603 根据考试中心ID获取试卷大类

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/classroom/versionm/center/getCenterPaperParts
```

需要传递以下参数：

| 字段     | 说明                                                         | 备注 |
| -------- | ------------------------------------------------------------ | ---- |
| centerID | 考试中心ID                                                   | 必选 |
| ltime    | 令牌时间戳                                                   | 必选 |
| pkey     | centerId、platformSource、version、time、salt、token加密而来的密钥 | 必选 |
| time     | 当前时间，格式为yyyy-MM-dd HH:mm:ss                          | 必选 |
| version  | 客户端版本号                                                 | 必选 |

返回数据包含如下字段：

| 字段          | 说明                             |
| ------------- | -------------------------------- |
| isNewQZ       |                                  |
| paperPartList | StudentPaperPart（试卷大类）集合 |
| code          | 结果代码，0为失败，1为成功       |

StudentPaperPart（试卷大类）包含如下字段：

| 字段         | 说明         |
| ------------ | ------------ |
| quesTypeID   | 未用到       |
| createTime   | 创建时间     |
| quesViewType |              |
| paperID      | 试卷ID       |
| sequence     |              |
| partName     | 试卷大类名称 |
| partID       | 试卷大类ID   |
| randomNum    |              |
| creator      | 创建者       |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "paperPartList": [
        {
            "quesTypeID": "",
            "createTime": 1613815651000,
            "quesViewType": 29183989,
            "paperID": null,
            "sequence": null,
            "partName": "计算分析题",
            "partID": 29183989,
            "randomNum": "",
            "creator": null
        }
    ],
    "code": 1
}
```

#### 10604 根据考试中心ID获取对外试卷

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/qzdownload/phone/paper/getCenterPaperViews
```

需要传递以下参数：

| 字段     | 说明                                                         | 备注 |
| -------- | ------------------------------------------------------------ | ---- |
| centerID | 考试中心ID                                                   | 必选 |
| ltime    | 令牌时间戳                                                   | 必选 |
| pkey     | centerId、platformSource、version、time、salt、token加密而来的密钥 | 必选 |
| time     | 当前时间，格式为yyyy-MM-dd HH:mm:ss                          | 必选 |
| version  | 客户端版本号                                                 | 必选 |

返回数据包含如下字段：

| 字段             | 说明                             |
| ---------------- | -------------------------------- |
| isNewQZ          | 未用到                           |
| questimeTypeList | StudentPaperView（对外试卷）集合 |
| code             | 结果代码，0为失败，1为成功       |
| msg              | 结果信息                         |

StudentPaperView（对外试卷）包含如下字段：

| 字段             | 说明                        |
| ---------------- | --------------------------- |
| paperType        | 试卷类型                    |
| createTime       | 创建时间                    |
| status           |                             |
| paperID          | 试卷ID                      |
| paperParam       |                             |
| paperOpenStatus  | 未用到                      |
| isContest        |                             |
| contestEndTime   |                             |
| dayiID           |                             |
| contestTimes     | 做题次数，大于0则有数次限制 |
| creator          | 创建者                      |
| modifyStatus     |                             |
| paperViewID      |                             |
| contestTimeLimit |                             |
| sequence         |                             |
| description      | 描述                        |
| explainURL       |                             |
| paperViewCatID   |                             |
| openStatus       |                             |
| paperViewName    |                             |
| contestStartTime |                             |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "questionTypeList": [
        {
            "paperType": 0,
            "createTime": "1604023276000",
            "status": 1,
            "paperID": 28992973,
            "paperParam": "",
            "paperOpenStatus": 0,
            "isContest": 0,
            "contestEndTime": "",
            "dayiID": 0,
            "contestTimes": 0,
            "creator": "10641",
            "modifyStatus": 0,
            "paperViewID": 28992974,
            "contestTimeLimit": 60,
            "sequence": 1,
            "description": "",
            "explainURL": "",
            "paperViewCatID": 0,
            "openStatus": 0,
            "paperViewName": "专题一 总论",
            "contestStartTime": ""
        }
    ],
    "code": 1,
    "msg": "成功"
}
```

#### 10605 获取试卷下的所有题目

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/qzdownload/phone/question/getPaperQuestionInfos
```

需要传递如下参数：

| 字段        | 说明                                                  | 备注 |
| ----------- | ----------------------------------------------------- | ---- |
| ltime       | 令牌时间戳                                            | 必选 |
| paperviewID |                                                       | 必选 |
| pkey        | paperViewID、version、time、salt、token加密而来的密钥 | 必选 |
| rowNumEnd   |                                                       | 可选 |
| rowNumStart |                                                       | 可选 |
| time        | 当前时间，格式为yyyy-MM-dd HH:mm:ss                   | 必选 |
| updateTime  | 更新时间                                              | 可选 |
| version     | 客户端版本号                                          | 必选 |

返回数据包含如下字段：

| 字段         | 说明                        |
| ------------ | --------------------------- |
| questionList | StudentQuestion（题目）集合 |
| isNewQZ      | 未用到                      |
| code         | 结果代码，0为失败，1为成功  |
| msg          | 结果消息                    |

StudentQuestion（题目）包含如下字段：

| 字段         | 说明     |
| ------------ | -------- |
| quesTypeID   |          |
| createTime   | 创建时间 |
| limitMinute  |          |
| lecture      |          |
| status       |          |
| questionID   |          |
| answer       | 答案     |
| score        |          |
| analysis     | 解析     |
| splitScore   |          |
| creator      | 创建人   |
| content      | 问题内容 |
| modifyStatus |          |
| parentID     |          |
| quesViewType |          |
| rowNum       | 未用到   |
| optNum       | 未用到   |

JSON格式的返回信息如下：

```json
{
    "questionList": [
        {
            "quesTypeID": 1,
            "createTime": "",
            "limitMinute": 120,
            "lecture": "",
            "status": 1,
            "questionID": 20085942,
            "answer": "D",
            "score": 1.5,
            "analysis": "可理解性要求企业提供的会计信息应当清晰明了，便于投资者等财务报告使用者理解和使用。",
            "splitScore": 0,
            "creator": 0,
            "content": "企业提供的会计信息应当清晰明了，便于财务会计报告使用者理解和使用，这句话体现的会计信息质量要求是（　）。",
            "modifyStatus": 0,
            "parentID": 0,
            "quesViewType": 1,
            "rowNum": 0,
            "optNum": 0
        }
    ],
    "isNewQZ": 1,
    "code": 1,
    "msg": "成功"
}
```

#### 10606 根据试卷ID获取其下的所有选项

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/qzdownload/phone/question/getPaperQuestionOptions
```

需要传递如下参数：

| 字段        | 说明                                                  | 备注 |
| ----------- | ----------------------------------------------------- | ---- |
| ltime       | 令牌时间戳                                            | 必选 |
| paperViewID |                                                       | 必选 |
| pkey        | paperViewID、version、time、salt、token加密而来的密钥 | 必选 |
| rowNumEnd   |                                                       | 可选 |
| rowNumStart |                                                       | 可选 |
| time        | 当前时间，格式为yyyy-MM-dd HH:mm:ss                   | 必选 |
| updateTime  | 更新时间                                              | 可选 |
| version     | 客户端版本号                                          | 必选 |

返回数据包含如下字段：

| 字段               | 说明                           |
| ------------------ | ------------------------------ |
| isNewQZ            | 未用到                         |
| questionOptionList | questionOption（问题选项）集合 |
| code               | 结果代码，0为失败，1为成功     |
| msg                | 结果消息                       |

StudentQuestionOption（问题选项）包含如下字段：

| 字段       | 说明       |
| ---------- | ---------- |
| questionID | 问题ID     |
| quesOption | 选项文本值 |
| quesValue  | 选项具体值 |
| sequence   |            |
| rowNum     | 未用到     |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "questionOptionList": [
        {
            "questionID": 20085942,
            "quesOption": "相关性",
            "quesValue": "A",
            "sequence": 1,
            "rowNum": 1
        }
    ],
    "code": 1,
    "msg": "成功"
}
```

#### 10607 保存做题记录

请注意与同步学习记录的区别，在提交卷子的时候触发调用，接口请求地址为：

```http
POST http://member.chinaacc.com/mapi/classroom/versionm/record/saveBatchMessage
```

需要传递如下参数：

| 字段        | 说明                                                         | 备注 |
| ----------- | ------------------------------------------------------------ | ---- |
| appkey      | 默认值：a5d25067-1f78-4a93-8060-6c6cb1492ef8                 | 必选 |
| guid        | 全局唯一GUID                                                 | 必选 |
| ltime       | 令牌时间戳                                                   | 必选 |
| online      |                                                              | 可选 |
| paperScores | 试卷分数                                                     | 必选 |
| pkey        | SsoUid、platformSource、version、time、type、guid、salt、token加密而成的密钥 | 必选 |
| time        | 当前时间，格式为yyyy-MM-dd HH:mm:ss                          | 必选 |
| type        | 默认值：qz                                                   | 必选 |
| uid         | SsoUid                                                       | 必选 |
| version     | 客户端版本号                                                 | 必选 |

返回数据包含如下字段：

| 字段      | 说明                       |
| --------- | -------------------------- |
| isNewQZ   |                            |
| finalList | StudentPaperFinalList集合  |
| code      | 结果代码，0为失败，1为成功 |

StudentPaperFinalList包含如下字段：

| 字段   | 说明                               |
| ------ | ---------------------------------- |
| guid   | 试卷的全局唯一ID（GUID），未用到   |
| result | StusentPaperResultFromSave集合     |
| code   | 结果代码，0为失败，1为成功，未用到 |

StudentPaperResultFromSave包含如下字段：

| 字段            | 说明   |
| --------------- | ------ |
| paperNewScoreID |        |
| paperOldScoreID | 未用到 |
| newQZSaveInfo   | 未用到 |

JSON格式的返回信息如下：

```json
{
    "isNewQZ": 1,
    "finalList": [
        {
            "guid": "dfa105fa-b0aa-45aa-894e-ad58d024dabe",
            "result": [
                {
                    "newQZSaveInfo": {
                        "statVer": null,
                        "status": null,
                        "creator": null,
                        "createTime": null,
                        "operator": null,
                        "operateTime": null,
                        "queryKey": null,
                        "bizID": 218591016,
                        "randomID": null,
                        "bizKey": "/chinaacc/qz/272/centerPaper/mobile",
                        "userID": 25441020,
                        "saveBizID": 28993132,
                        "paperViewID": null,
                        "bizType": 2,
                        "recordDesc": "[2021年预习阶段练习] 专题三 负债",
                        "paperParam": null,
                        "saveBizIDs": null,
                        "version": null,
                        "allQuesNum": 1,
                        "doneQuesNum": 1,
                        "rightQuesNum": 0,
                        "undoneQuesNum": 0,
                        "errorQuesNum": 0,
                        "autoNum": 1,
                        "autoRightNum": 0,
                        "isDoneText": 0,
                        "isShowApp": 1,
                        "isShowPC": 1,
                        "useShowPC": null,
                        "paperCatParam": null,
                        "examName": "专题三 负债",
                        "contestTimeLimit": 120,
                        "isTemp": 0,
                        "spendTime": 22,
                        "lastScore": 0,
                        "autoScore": 0,
                        "doneScore": 2,
                        "synError": false
                    },
                    "paperNewScoreID": 218591016,
                    "paperOldScoreID": "0"
                }
            ],
            "code": "1"
        }
    ],
    "code": "1"
}
```

#### 10608 从线上获取做题记录信息

接口请求地址为：

```http
GET http://member.chinaacc.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch
```

需要传递如下参数:

| 字段          | 说明                                                         | 备注 |
| ------------- | ------------------------------------------------------------ | ---- |
| couseID       |                                                              | 可选 |
| ltime         | 令牌时间戳                                                   | 必选 |
| paperScoreIDs | 七天内做题记录ID                                             | 必选 |
| pkey          | SsoUid、platformSource、version、time、salt、token加密而成的密钥 | 必选 |
| time          | 当前时间，格式为yyyy-MM-dd HH:mm:ss                          | 必选 |
| userID        | 用户ID                                                       | 必选 |
| version       | 客户端版本号                                                 | 必选 |

返回数据包含如下字段：

| 字段        | 说明                       |
| ----------- | -------------------------- |
| code        | 结果代码，0为失败，1为成功 |
| paperScores | StudentPaperScore集合      |

StudentPaperScore包含如下字段：

| 字段         | 说明                               |
| ------------ | ---------------------------------- |
| paperScoreID | 默认为0                            |
| autoScore    | 试卷得分                           |
| centerID     | 考试中心ID                         |
| createTime   | 做题时间                           |
| lastScore    | 试卷得分                           |
| paperScore   | 试卷总分                           |
| paperviewID  | 对外试卷ID                         |
| siteCourseID | 对外课程ID                         |
| spendTime    | 做题用时                           |
| userID       | 用户ID                             |
| answers      | StudentPaperAnswer（用户答案）集合 |

StudentPaperScoreAnswer（用户答案）包含如下字段：

| 字段         | 说明     |
| ------------ | -------- |
| paperScoreID | 默认为0  |
| questionID   | 题目ID   |
| userAnswer   | 用户答案 |
| userScore    | 用户得分 |

JSON格式的返回信息如下:

```json
{
    "paperScores": [],
    "code": "1"
}
```