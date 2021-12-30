namespace Framework.Remote
{
    internal static class Interface
    {
        #region 公共接口定义

        internal const string PlatformSource = "10";
        /**获取当前服务器时间
         * @author ChW
         * @date 2021-06-17
         */
        internal const string GetCurrentTimeUrl_2021 = "+/server/time";
        /**获取学员跑马灯信息 新接口10102
         * @author ChW
         * @date 2021-06-15
         */
        internal const string StudentGetMarqueeInfoListUrl_2021 = "+/dlclassroom/getMarqueeInfoListGateway";
        /**账户登录 新接口10203
         * @author ChW
         * @date 2021-06-16
         */
        internal const string StudentLoginUrl_2021 = "+/appLogin/downUserLogin"; // "+/appLogin/userLogin";

        /// <summary>
        /// 用户登录
        /// </summary>
        internal const string StudentLoginUrl = "http://portal.cdeledu.com/interface/login.php";

        /// <summary>
        /// 踢设备接口(无用 dgh 2018.01.05)
        /// </summary>
        internal const string StudentKickDevice = "http://portal.cdeledu.com/interface/changeMid.php";

        /// <summary>
        /// 获取下载IP地址
        /// </summary>
        internal const string GetDownloadIpUrl = "http://manage.mobile.cdeledu.com/analysisApi/getDownIp.shtm";

        /// <summary>
        /// 上传用户使用信息接口
        /// </summary>
        internal const string UploadBigDataLogUrl = "http://data.cdeledu.com/SaveFileData";

        /// <summary>
        /// 通知到达接口
        /// </summary>
        internal const string SavePcPushInfoUrl = "http://manage.mobile.cdeledu.com/analysisApi/push/savePCPushInfo.shtm";

        /// <summary>
        /// 检查用户最后登录SID
        /// </summary>
        internal const string StudentCheckSid = "http://portal.cdeledu.com/api/index.php";

        /// <summary>
        /// 检查用户是否被屏蔽(无用 dgh 2018.01.05)
        /// </summary>
        internal const string StudentCheckAbleUrl = "http://manage.mobile.cdeledu.com/analysisApi/dlclassroom/checkUseable.shtm";

        /// <summary>
        /// 获取学员跑马灯信息
        /// </summary>
        internal const string StudentGetMarqueeInfoListUrl = "http://manage.mobile.cdeledu.com/analysisApi/dlclassroom/getMarqueeInfoList.shtm";

        /// <summary>
        /// 获取可用的离线时长(无用 dgh 2018.01.05)
        /// </summary>
        internal const string StudentGetOfflineUseTimeUrl = "http://manage.mobile.cdeledu.com/analysisApi/downloadclass/getOffLineUseTime.shtm";

        /// <summary>
        /// 强制登录接口
        /// </summary>
        internal const string StudentGotoUrlLoginUrl = "http://portal.cdeledu.com/interface/ucChangePoint.php?sid={0}&ssouid={1}&pkey={2}&memberlevel={4}&memberkey={5}&time={6}&turl={3}";

        /// <summary>
        /// 获取当前服务器时间
        /// </summary>
        internal const string GetCurrentTimeUrl = "http://portal.cdeledu.com/interface/ucGetSTime.php";
        #endregion

        #region 会计网接口地址定义
#if CHINAACC
        /**网关
         * @author ChW
         * @date 2021-05-27
         */
        internal const string gateway = "http://gateway.cdeledu.com/doorman/op";
        /**网校
         * @author ChW
         * @date 2021-06-07
         */
        internal const string domain = "chinaacc";
        /**aes加密相关
         * @author ChW
         * @date 2021-05-27
         */
        internal const string aesKey = "823s4125660ijf;*";
        internal const string ivKey = "qyu148#4(1p_1^4;";
        /**账户冻结检测 新接口10202
         * @author ChW
         * @date 2021-06-15
         */
        internal const string CheckUserFrozenUrl_2021 = "+/appLogin/ucChkUserLogin";
        /**获取token访问令牌 新接口10205
         * @author ChW
         * @date 2021-06-07
         */
        internal const string GetToken_2021 = "+/auth/token/getTokenGateway";
        /**上传（同步)学习记录 新接口10301、10607
         * @author ChW
         * @date 2021-06-07
         */
        internal const string SaveBatchMessageUrl_2021 = "+/classroom/versionm/record/saveBatchMessageGateway";
        /**获取班次类别列表 新接口10401
         * @author ChW
         * @date 2021-06-15
         */
        internal const string GetUserCourseListUrl_2021 = "+/versionm/classroom/mycware/getUserCourseListNewGateway";
        /**获取班次列表 新接口10402
         * @author ChW
         * @date 2021-06-15
         */
        internal const string GetUserWareBySubjectIdUrl_2021 = "+/versionm/classroom/mycware/getUserWareBySubjectIDGateway";
        /**获取班次内的章节、讲次信息 新接口10403
         * @author ChW
         * @date 2021-06-07
         */
        internal const string GetStudentCourseDetailMemberUrl_2021 = "+/mapi/versionm/classroom/course/getCourseDetailGw";
        /**获取视频密钥 新接口10405
         * @author ChW
         * @date 2021-05-24
         */
        internal const string GetStudentCwareKeyUrl_2021 = "+/versionm/classroom/course/getCwareKeyGateway";
        /**获取视频对应知识点测试及弹出时间 新接口10408
         * @author ChW
         * @date 2021-06-07
         */
        internal const string GetPointTestStartTimeUrl_2021 = "+/classroom/versionm/cware/getPointTestStartTimeGateway";
        /**获取知识点测试的题目信息 新接口10409
         * @author ChW
         * @date 2021-06-08
         */
        internal const string GetQuestionByPointTestIdUrl_2021 = "+/classroom/versionm/record/getQuestionByPointTestIDGateway";
        /**上传知识点测试的做题结果 新接口10410
         * @author ChW
         * @date 2021-06-08
         */
        internal const string SavePointTestQzResultUrl_2021 = "+/classroom/versionm/record/savePointTestQzResultGateway";
        /**同步视频观看记录 新接口10411
         * @author ChW
         * @date 2021-06-08
         */
        internal const string SaveNextBeginTime_2021 = "+/versionm/classroom/course/saveNextBeginTimeGateway";
        /**获取课程讲义下载接口 新接口10503
         * @author ChW
         * @date 2021-06-16
         */
        internal const string GetSmallListKcjyUrl_2021 = "+/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrlGw";
        /**根据科目id获取考试中心列表 新接口10601
         * @author ChW
         * @date 2021-06-08
         */
        internal const string GetPaperCentersBySubjectIdUrl_2021 = "+/classroom/versionm/center/getPaperCentersBySubjectIDGateway";
        /**根据考试中心id获取试卷列表 新接口10602
         * @author ChW
         * @date 2021-06-08
         */
        internal const string GetQzCenterPapersUrl_2021 = "+/classroom/versionm/center/getNewCenterPapersGateway";
        /**根据考试中心id获取试卷大类 新接口10603
         * @author ChW
         * @date 2021-06-08
         */
        internal const string GetQzCenterPaperPartsUrl_2021 = "+/classroom/versionm/center/getCenterPaperPartsGateway";
        /**根据考试中心id获取对外试卷 新接口10604
         * @author ChW
         * @date 2021-06-09
         */
        internal const string GetQzCenterPaperViewsUrl_2021 = "+/mapi/qzdownload/phone/paper/getCenterPaperViewsGateway";
        /**获取试卷下的所有题目 新接口10605
         * @author ChW
         * @date 2021-06-10
         */
        internal const string GetQzPaperQuestionInfoUrl_2021 = "+/mapi/qzdownload/phone/question/getPaperQuestionInfosGateway";
        /**根据试卷id获取其下的所有选项 新接口10606
         * @author ChW
         * @date 2021-06-10
         */
        internal const string GetQzPaperQuestionOptionsUrl_2021 = "+/mapi/qzdownload/phone/question/getPaperQuestionOptionsGateway";
        /** 从线上获取做题记录信息 新接口10608
         * @author ChW
         * @date 2021-06-11
         */
        internal const string GetPaperScoreBatchUrl_2021 = "+/mapi/classroom/versionm/record/syncClouderPaperScoreBatchGateway";
        /**获取已绑定设备列表 新接口10206
         * @author ChW
         * @date 2021-06-22
         */
        internal const string GetBindedDeviceListUrl_2021 = "+/appLogin/bindEquipment";
        /**发送验证码 新接口10207
         * @author ChW
         * @date 2021-06-25
         */
        internal const string SendVerificationCodeUrl_2021 = "+/appLogin/sendMessage";
        /**使用验证码效验身份 新接口10208
         * @author ChW
         * @date 2021-06-25
         */
        internal const string CheckUserIdentityByVerificationCodeUrl_2021 = "+/appLogin/checkAuthCode";
        /**解绑设备 新接口10209
         * @author ChW
         * @date 2021-06-25
         */
        internal const string UnbindDeviceUrl_2021 = "+/appLogin/equipmentOut";
        /**绑定手机号 新接口10210
         * @author ChW
         * @date 2021-07-01
         */
        internal const string BindPhoneUrl_2021 = "+/appLogin/userAccountBind";

        /// <summary>
        /// 所处的网校
        /// </summary>
        internal const string Domain = "@chinaacc.com";

        /// <summary>
        /// 应用编号
        /// </summary>
        internal const short AppId = 303;
        /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "a5d25067-1f78-4a93-8060-6c6cb1492ef8";

        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.chinaacc.com/mapi/auth/token/getToken";

        /// <summary>
        /// 检查用户是否被冻结
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.chinaacc.com/mapi/versionm/classroom/member/getUsserWaring";

        /// <summary>
        /// 获取用户所报课程所属科目信息
        /// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.chinaacc.com/mapi/versionm/classroom/mycware/getUserCourseList";

        /// <summary>
        /// 通过购买课程获取课件信息
        /// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.chinaacc.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";

        /// <summary>
        /// 获取用户购买课件的章节
        /// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.chinaacc.com/mapi/versionm/classroom/course/getCourseDetail";
       
        /// <summary>
        /// (老接口)无用可去掉 dgh 2018.01.05
        /// </summary>
        internal const string GetStudentCwareKeyUrl = "http://mportal.chinaacc.com/Course/getCwareKey";

        /// <summary>
        /// 视频加密
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.chinaacc.com/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.chinaacc.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";

        /// <summary>
        /// 获取所有题型
        /// </summary>
        //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.chinaacc.com/mapi/qzdownload/phone/questiontype/getQueType";

        /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.chinaacc.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.chinaacc.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

        
        /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.chinaacc.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.chinaacc.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.chinaacc.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

        /// <summary>
        /// 获取分录题借贷关系数据
        /// </summary>
        //新接口 dgh 2018.01.16
        internal const string GetSubjectClassifyUrlNew = "http://member.chinaacc.com/mapi/qzdownload/phone/question/getSubjectClassify";
       
        /// <summary>
        /// 获取课程讲义下载接口
        /// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.chinaacc.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.chinaacc.com/video/header/Visitor";

        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.chinaacc.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.chinaacc.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.chinaacc.com/video/header/GetHeader";

        /// <summary>
        /// 获取课件视频对应的知识点测试及弹出时间
        /// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.chinaacc.com/mapi/classroom/versionm/cware/getPointTestStartTime";

        /// <summary>
        /// 获取知识点测（练习）试题目信息
        /// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.chinaacc.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

        /// <summary>
        /// 保存知识点测试（练习）做题结果
        /// </summary>
        //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.chinaacc.com/mapi/classroom/versionm/record/savePointTestQzResult";

        /// <summary>
        /// 获取用户资料（只有方法无调用 dgh 2018.01.08）
        /// </summary>
        internal const string GetUserDataUrl = "http://member.chinaacc.com/mobile/classroom/member/getUserData.shtm";

        /// <summary>
        /// 开通七天内下载  dgh 2015.06.23
        /// </summary>
        internal const string StudentGetOpenCourseDownloadUrl = "http://member.chinaacc.com/mapi/versionm/classroom/course/getOpenCourseDownload";

        /// <summary>
        /// 跳转到答疑板地址
        /// </summary>
        //internal const string StudentFaqUrl = "http://member.chinaacc.com/faq/myIndex.shtm";
        internal const string StudentFaqUrl = "http://member.chinaacc.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.chinaacc.com%2Ffaq%2FmyIndex.shtm";

        /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.chinaacc.com/faq/query.shtm?cwareID={0}&qclass=b0015&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.chinaacc.com/member/loginDispose.shtm?gotoURL={0}";
        /// <summary>
        /// 恢复设备
        /// </summary>
        internal const string DeviceUrl = "http://member.chinaacc.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";

        /// <summary>
        /// 保存做题记录
        /// </summary>
        internal const string SaveBatchMessageUrl = "http://member.chinaacc.com/mapi/classroom/versionm/record/saveBatchMessage";
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
        internal const string GetPaperScoreBatchUrl = "http://member.chinaacc.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";

        /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.chinaacc.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = "http://member.chinaacc.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";

        /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.chinaacc.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fwww.chinaacc.com%2Fselcourse%2Findex.shtml";
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.chinaacc.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.chinaacc.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 医学网

#if MED
		/// <summary>
		/// 所处的网校
		/// </summary>
        internal const string Domain = "@med66.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 304;
        /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "a927a05d-02ca-43f5-946d-089120f18173";

        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.med66.com/mapi/auth/token/getToken";
		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.med66.com/mapi/versionm/classroom/member/getUsserWaring";

		/// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
      //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.med66.com/mapi/versionm/classroom/mycware/getUserCourseList";

		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
          //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.med66.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";

		/// <summary>
        /// 获取用户购买课件的章节
        /// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.med66.com/mapi/versionm/classroom/course/getCourseDetail";


        internal const string GetStudentCwareKeyUrl = "http://mportal.med66.com/Course/getCwareKey";
        //视频加密
        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.med66.com/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.med66.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";

		
         /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.med66.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.med66.com/mapi/qzdownload/phone/questiontype/getQueType";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.med66.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

         /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.med66.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.med66.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.med66.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.med66.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.med66.com/video/header/Visitor";
        
        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.med66.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.med66.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.med66.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
		internal const string GetPointTestStartTimeUrl = "http://member.med66.com/newApi/classroom/qzexam/cware/getPointTestStartTime.shtm";
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.med66.com/mapi/classroom/versionm/cware/getPointTestStartTime";

		/// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
       //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.med66.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.med66.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.med66.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
         internal const string StudentGetOpenCourseDownloadUrl = "http://member.med66.com/mapi/versionm/classroom/course/getOpenCourseDownload";

		/// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		//internal const string StudentFaqUrl = "http://member.med66.com/faq/myIndex.shtm";
		internal const string StudentFaqUrl = "http://member.med66.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.med66.com%2Ffaq%2FmyIndex.shtm";

        /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.med66.com/faq/query.shtm?Forum_ID={0}&qclass=b0001&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.med66.com/member/loginDispose.shtm?gotoURL={0}";
         /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.med66.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";
         /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.med66.com/mapi/classroom/versionm/record/saveBatchMessage";
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.med66.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
         /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.med66.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.med66.com%2Fselcourse%2Findex.shtm";
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.med66.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
         /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.med66.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = "http://member.med66.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.med66.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 建设网

#if JIANSHE
	    ///<summary>
        ///所处的网校
        ///</summary>
        internal const string Domain = "@jianshe99.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 305;
         /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "28ca4489-fa8f-4fa3-aa84-b3d4bc8060b0";

        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.jianshe99.com/mapi/auth/token/getToken";
		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.jianshe99.com/mapi/versionm/classroom/member/getUsserWaring";
       
		/// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.jianshe99.com/mapi/versionm/classroom/mycware/getUserCourseList";

		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.jianshe99.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";

        /// <summary>
        /// 获取用户购买课件的章节
        /// </summary>
         internal const string GetStudentCourseDetailMemberUrlNew = "http://member.jianshe99.com/mapi/versionm/classroom/course/getCourseDetail";

        internal const string GetStudentCwareKeyUrl = "http://mportal.jianshe99.com/Course/getCwareKey";

        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.jianshe99.com/mapi/versionm/classroom/course/getCwareKey";

		
        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.jianshe99.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";

        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.jianshe99.com/mapi/qzdownload/phone/questiontype/getQueType";

         /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.jianshe99.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.jianshe99.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

         /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.jianshe99.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.jianshe99.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.jianshe99.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.jianshe99.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.jianshe99.com/video/header/Visitor";
        
        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.jianshe99.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.jianshe99.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.jianshe99.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
         //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.jianshe99.com/mapi/classroom/versionm/cware/getPointTestStartTime";

		/// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.jianshe99.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.jianshe99.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.jianshe99.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
         internal const string StudentGetOpenCourseDownloadUrl = "http://member.jianshe99.com/mapi/versionm/classroom/course/getOpenCourseDownload";

		/// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		//internal const string StudentFaqUrl = "http://member.jianshe99.com/faq/myIndex.shtm";
		internal const string StudentFaqUrl = "http://member.jianshe99.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.jianshe99.com%2Ffaq%2FmyIndex.shtm";

         /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.jianshe99.com/faq/query.shtm?Forum_ID={0}&qclass=b0002&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.jianshe99.com/member/loginDispose.shtm?gotoURL={0}";
        //internal const string LoginQueryUrl = "http://member.jianshe99.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.jianshe99.com%2Ffaq%2Fquery.shtm%3FForum_ID={0}&qclass=b0002%26QNo={1}&Jy_url={2}";
         /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.jianshe99.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";
         /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.jianshe99.com/mapi/classroom/versionm/record/saveBatchMessage";
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.jianshe99.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
        /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.jianshe99.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fwww.jianshe99.com%2Fselcourse%2Findex.shtml";
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.jianshe99.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
         /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.jianshe99.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = " http://member.jianshe99.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.jianshe99.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 法律网

#if LAW
	/// <summary>
	/// 所处的网校
	/// </summary>
        internal const string Domain = "@chinalawedu.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 306;
         /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "828d9a5d-354c-45eb-a38c-b17384cdac84";
         /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.chinalawedu.com/mapi/auth/token/getToken";

		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.chinalawedu.com/mapi/versionm/classroom/member/getUsserWaring";
        
        /// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.chinalawedu.com/mapi/versionm/classroom/mycware/getUserCourseList";

		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.chinalawedu.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";

        /// <summary>
        /// 获取用户购买课件的章节
        /// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.chinalawedu.com/mapi/versionm/classroom/course/getCourseDetail";

        internal const string GetStudentCwareKeyUrl = "http://mportal.chinalawedu.com/Course/getCwareKey";

        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.chinalawedu.com/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.chinalawedu.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";

        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.chinalawedu.com/mapi/qzdownload/phone/questiontype/getQueType";

        /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.chinalawedu.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
       //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.chinalawedu.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

         /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.chinalawedu.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.chinalawedu.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.chinalawedu.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.chinalawedu.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.chinalawedu.com/video/header/Visitor";
        
        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.chinalawedu.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.chinalawedu.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.chinalawedu.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.chinalawedu.com/mapi/classroom/versionm/cware/getPointTestStartTime";

		/// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.chinalawedu.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.chinalawedu.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.chinalawedu.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
         internal const string StudentGetOpenCourseDownloadUrl = "http://member.chinalawedu.com/mapi/versionm/classroom/course/getOpenCourseDownload";

		/// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		//internal const string StudentFaqUrl = "http://member.chinalawedu.com/faq/myIndex.shtm";
		internal const string StudentFaqUrl = "http://member.chinalawedu.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.chinalawedu.com%2Ffaq%2FmyIndex.shtm";
        /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.chinalawedu.com/faq/query.shtm?Forum_ID={0}&qclass=b0003&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.chinalawedu.com/member/loginDispose.shtm?gotoURL={0}";
         /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.chinalawedu.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";

         /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.chinalawedu.com/mapi/classroom/versionm/record/saveBatchMessage";
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.chinalawedu.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";

        /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.chinalawedu.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";

        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = "http://member.chinalawedu.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";

        /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.chinalawedu.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.chinalawedu.com%2Fselcourse%2Findex.shtm";

        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.chinalawedu.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
        
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.chinalawedu.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 职教网

#if CHINATAT
	    /// <summary>
	    /// 所处的网校
	    /// </summary>
        internal const string Domain = "@chinatat.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 308;
         /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "b04b6600-5aa2-4be8-b8db-f1f1761825ef";

        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.chinatat.com/mapi/auth/token/getToken";

		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.chinatat.com/mapi/versionm/classroom/member/getUsserWaring";
       
        /// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.chinatat.com/mapi/versionm/classroom/mycware/getUserCourseList";

		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.chinatat.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";

        /// <summary>
        /// 获取用户购买课件的章节
        /// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.chinatat.com/mapi/versionm/classroom/course/getCourseDetail";

        internal const string GetStudentCwareKeyUrl = "http://mportal.chinatat.com/Course/getCwareKey";

        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.chinatat.com/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.chinatat.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";

       
        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.chinatat.com/mapi/qzdownload/phone/questiontype/getQueType";

        /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.chinatat.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.chinatat.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

        /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.chinatat.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.chinatat.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.chinatat.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.chinatat.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.chinatat.com/video/header/Visitor";
        
        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.chinatat.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.chinatat.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.chinatat.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.chinatat.com/mapi/classroom/versionm/cware/getPointTestStartTime";
		
        /// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.chinatat.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.chinatat.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.chinatat.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
         internal const string StudentGetOpenCourseDownloadUrl = "http://member.chinatat.com/mapi/versionm/classroom/course/getOpenCourseDownload";

		/// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		//internal const string StudentFaqUrl = "http://member.chinatat.com/faq/myIndex.shtm";
		internal const string StudentFaqUrl = "http://member.chinatat.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.chinatat.com%2Ffaq%2FmyIndex.shtm";

        /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.chinatat.com/faq/query.shtm?Forum_ID={0}&qclass=b0001&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.chinatat.com/member/loginDispose.shtm?gotoURL={0}";
         /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.chinatat.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";

         /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.chinatat.com/mapi/classroom/versionm/record/saveBatchMessage";
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.chinatat.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
         /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.chinatat.com/selectcourse/index.shtm";
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.chinatat.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
         /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.chinatat.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = "http://member.chinatat.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.chinatat.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 中小学

#if G12E
	     /// <summary>
	     /// 所处的网校
	     /// </summary>
        internal const string Domain = "@g12e.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 309;
        /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "5aee2528-60a4-41c3-8d13-ef8dd511eaf3";

        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.g12e.com/mapi/auth/token/getToken";

		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.g12e.com/mapi/versionm/classroom/member/getUsserWaring";
        
        /// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.g12e.com/mapi/versionm/classroom/mycware/getUserCourseList";

		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.g12e.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";
        
        /// <summary>
        /// 获取用户购买课件的章节
        /// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.g12e.com/mapi/versionm/classroom/course/getCourseDetail";

        internal const string GetStudentCwareKeyUrl = "http://mportal.g12e.com/Course/getCwareKey";
		
        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.g12e.com/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.g12e.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";

        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.g12e.com/mapi/qzdownload/phone/questiontype/getQueType";

         /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.g12e.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.g12e.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

        /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.g12e.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.g12e.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.g12e.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.g12e.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.g12e.com/video/header/Visitor";
        
        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.g12e.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.g12e.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.g12e.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.g12e.com/mapi/classroom/versionm/cware/getPointTestStartTime";
		/// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.g12e.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.g12e.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.g12e.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
         internal const string StudentGetOpenCourseDownloadUrl = "http://member.g12e.com/mapi/versionm/classroom/course/getOpenCourseDownload";

        /// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		internal const string StudentFaqUrl = "http://member.g12e.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.g12e.com%2Ffaq%2FmyIndex.shtm";

        /// <summary>
        /// 播放/讲义的提问
        /// </summary>
        internal const string queryUrl = "http://member.g12e.com/faq/query.shtm?Forum_ID={0}&qclass=b0015&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.g12e.com/member/loginDispose.shtm?gotoURL={0}";
         /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.g12e.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";

         /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.g12e.com/mapi/classroom/versionm/record/saveBatchMessage";
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.g12e.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
         /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.g12e.com/selcourse/index.shtm";
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.g12e.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
         /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.g12e.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = " http://member.g12e.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.g12e.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 自考

#if ZIKAO
	/// <summary>
	/// 所处的网校
	/// </summary>
        internal const string Domain = "@zikao365.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 311;
         /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "16af376a-0ff6-41d5-9f76-75f316c9b514";

        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.zikao365.com/mapi/auth/token/getToken";

		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.zikao365.com/mapi/versionm/classroom/member/getUsserWaring";
       
		/// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.zikao365.com/mapi/versionm/classroom/mycware/getUserCourseList";

		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.zikao365.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";
		
        /// <summary>
		/// 获取用户购买课件的章节
		/// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.zikao365.com/mapi/versionm/classroom/course/getCourseDetail";

		internal const string GetStudentCwareKeyUrl = "http://mportal.zikao365.com/Course/getCwareKey";

        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.zikao365.com/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.zikao365.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";

        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.zikao365.com/mapi/qzdownload/phone/questiontype/getQueType";

         /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.zikao365.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.zikao365.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

         /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.zikao365.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.zikao365.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.zikao365.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.zikao365.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

		/// <summary>
		/// 记录访问者
		/// </summary>
		internal const string RecordUserUrl = "http://mportal.zikao365.com/video/header/Visitor";

		/// <summary>
		/// 产生密钥
		/// </summary>
		internal const string GetGenKeyUrl = "http://mportal.zikao365.com/video/header/genkey";
		/// <summary>
		/// 获取文件头
		/// </summary>
		internal const string GetVideoHeadUrl = "http://mportal.zikao365.com/video/header/GetHead";

		/// <summary>
		/// 直接获取文件头
		/// </summary>
		internal const string GetVideoHeaderUrl = "http://mportal.zikao365.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.zikao365.com/mapi/classroom/versionm/cware/getPointTestStartTime";
		
        /// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.zikao365.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.zikao365.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.zikao365.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
         internal const string StudentGetOpenCourseDownloadUrl = "http://member.zikao365.com/mapi/versionm/classroom/course/getOpenCourseDownload";

        /// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		internal const string StudentFaqUrl = "http://member.zikao365.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.zikao365.com%2Ffaq%2FmyIndex.shtm";

         /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.zikao365.com/faq/query.shtm?Forum_ID={0}&qclass=b0015&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.zikao365.com/member/loginDispose.shtm?gotoURL={0}";
         /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.zikao365.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";
         /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.zikao365.com/mapi/classroom/versionm/record/saveBatchMessage";
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.zikao365.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
         /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.zikao365.com/selcourse/index.shtm";
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.zikao365.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
         /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.zikao365.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = " http://member.zikao365.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.zikao365.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 外语

#if FOR68
        /// <summary>
	    /// 所处的网校
	    /// </summary>
        internal const string Domain = "@for68.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 307;
        /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "d215febc-4c2a-4629-87a7-77f9392ce934";

        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.for68.com/mapi/auth/token/getToken";

		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.for68.com/mapi/versionm/classroom/member/getUsserWaring";
       
		/// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.for68.com/mapi/versionm/classroom/mycware/getUserCourseList";
		
        /// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.for68.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";
		
        /// <summary>
		/// 获取用户购买课件的章节
		/// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.for68.com/mapi/versionm/classroom/course/getCourseDetail";

		internal const string GetStudentCwareKeyUrl = "http://mportal.for68.com/Course/getCwareKey";
		
        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.for68.com/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.for68.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";
       
        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.for68.com/mapi/qzdownload/phone/questiontype/getQueType";

         /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.for68.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.for68.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

        /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.for68.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.for68.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.for68.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.for68.com/video/header/Visitor";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.for68.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.for68.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.for68.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.for68.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.for68.com/mapi/classroom/versionm/cware/getPointTestStartTime";

		/// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.for68.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.for68.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.for68.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
         internal const string StudentGetOpenCourseDownloadUrl = "http://member.for68.com/mapi/versionm/classroom/course/getOpenCourseDownload";

        /// <summary>
        /// 跳转到答疑板地址
        /// </summary>
        internal const string StudentFaqUrl = "http://member.for68.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.for68.com%2Ffaq%2FmyIndex.shtm";

         /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.for68.com/faq/query.shtm?Forum_ID={0}&qclass=b0015&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.for68.com/member/loginDispose.shtm?gotoURL={0}";
        
        /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.for68.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";
         
        /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.for68.com/mapi/classroom/versionm/record/saveBatchMessage";
        
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.for68.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
        
        /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.for68.com//selcourse/index.shtm";
        
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.for68.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
         
        /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.for68.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = "http://member.for68.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";
        
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.for68.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 考研

#if KAOYAN
	/// <summary>
	/// 所处的网校
	/// </summary>
        internal const string Domain = "@chinaacc.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 310;
        /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "";

		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.cnedu.cn/mapi/versionm/classroom/member/getUsserWaring";
       
        /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.cnedu.com/mapi/auth/token/getToken";

		/// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.cnedu.cn/mapi/versionm/classroom/mycware/getUserCourseList";

		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.cnedu.cn/mapi/versionm/classroom/mycware/getUserWareBySubjectID";
		
        /// <summary>
		/// 获取用户购买课件的章节
		/// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.cnedu.cn/mapi/versionm/classroom/course/getCourseDetail";

		internal const string GetStudentCwareKeyUrl = "http://mportal.chinaacc.cn/Course/getCwareKey";
		
        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.cnedu.cn/mapi/versionm/classroom/course/getCwareKey";

        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.cnedu.cn/mapi/classroom/versionm/center/getPaperCentersBySubjectID";
        
        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.cnedu.cn/mapi/qzdownload/phone/questiontype/getQueType";

         /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.cnedu.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.cnedu.cn/mapi/qzdownload/phone/paper/getCenterPaperViews";

         /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.cnedu.cn/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.cnedu.cn/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.cnedu.cn/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
        //新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.cnedu.cn/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

        /// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.chinaacc.com/video/header/Visitor";

        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.chinaacc.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.chinaacc.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.chinaacc.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.cnedu.cn/mapi/classroom/versionm/cware/getPointTestStartTime";

		/// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.cnedu.cn/mapi/classroom/versionm/record/getQuestionByPointTestID";
		
        /// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.cnedu.cn/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.cnedu.cn/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
        internal const string StudentGetOpenCourseDownloadUrl = "http://member.cnedu.cn/mapi/versionm/classroom/course/getOpenCourseDownload";

		/// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		//internal const string StudentFaqUrl = "http://member.cnedu.cn/faq/myIndex.shtm";
		internal const string StudentFaqUrl = "http://member.cnedu.cn/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.cnedu.cn%2Ffaq%2FmyIndex.shtm";

        /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.cnedu.cn/faq/query.shtm?Forum_ID={0}&qclass=b0015&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://member.cnedu.cn/member/loginDispose.shtm?gotoURL={0}";
         
        /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.cnedu.cn/mobilewap/wap/bangdingpc/cancleEquipment.shtm";
         
        /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.cnedu.cn/mapi/classroom/versionm/record/saveBatchMessage";
        
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.cnedu.cn/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
       
        /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.cnedu.cn/selectcourse/index.shtm";
        
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.cnedu.cn/mapi/classroom/versionm/paper/getPaperSubmitCnt";
        
        /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.cnedu.cn/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = " http://member.cnedu.cn/mobilewap/wap/downloadClass/downloadClassInit.shtm";
        
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.cnedu.cn/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

        #region 成考

#if CHENGKAO
	   /// <summary>
	   /// 所处的网校
	  /// </summary>
        internal const string Domain = "@zikao365.com";

		/// <summary>
		/// 应用编号
		/// </summary>
		internal const short AppId = 312;

        /// <summary>
        /// 新接口AppKey dgh 2016.11.02
        /// </summary>
        internal const string AppKey = "";

         /// <summary>
        /// 新接口获取令牌码 dgh 2016.11.02
        /// </summary>
        internal const string GetToken = "http://member.chengkao365.com/mapi/auth/token/getToken";

		/// <summary>
		/// 检查用户是否被冻结
		/// </summary>
        //新接口 dgh 2018.01.15
        internal const string CheckUserFrozenUrlNew = "http://member.chengkao365.com/mapi/versionm/classroom/member/getUsserWaring";

        /// <summary>
		/// 获取用户所报课程所属科目信息
		/// </summary>
        //新接口 dgh 2018.01.02
        internal const string GetUserCourseListUrlNew = "http://member.chengkao365.com/mapi/versionm/classroom/mycware/getUserCourseList";
		/// <summary>
		/// 通过购买课程获取课件信息
		/// </summary>
         //新接口 dgh 2018.01.02
        internal const string GetUserWareBySubjectIdUrlNew = "http://member.chengkao365.com/mapi/versionm/classroom/mycware/getUserWareBySubjectID";
		
        /// <summary>
		/// 获取用户购买课件的章节
		/// </summary>
        internal const string GetStudentCourseDetailMemberUrlNew = "http://member.cehengkao365.com/mapi/versionm/classroom/course/getCourseDetail";

        internal const string GetStudentCwareKeyUrl = "http://mportal.cehengkao365.com/Course/getCwareKey";

        //新接口 dgh 2018.01.15
        internal const string GetStudentCwareKeyUrlNew = "http://member.cehengkao365.com/mapi/versionm/classroom/course/getCwareKey";
        
        /// <summary>
        /// 根据科目ID获取考试中心(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetPaperCentersBySubjectIdUrlNew = "http://member.chengkao365.com/mapi/classroom/versionm/center/getPaperCentersBySubjectID";
        
        /// <summary>
        /// 获取所有题型
        /// </summary>
         //新接口 dgh 2018.01.16
        internal const string GetQzQuestionTypeUrlNew = "http://member.chengkao365.com/mapi/qzdownload/phone/questiontype/getQueType";

         /// <summary>
        /// 获取所有考试中心ID为1的试卷信息(新接口 dgh 2018.01.12)
        /// </summary>
        internal const string GetQzCenterPapersUrlNew = "http://member.chengkao365.com/mapi/classroom/versionm/center/getNewCenterPapers";

        /// <summary>
        /// 获取所有考试中心下的对外试卷
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzCenterPaperViewsUrlNew = "http://member.chengkao365.com/mapi/qzdownload/phone/paper/getCenterPaperViews";

         /// <summary>
        /// 获取试卷的大类(新接口 dgh 2018.01.15)
        /// </summary>
        internal const string GetQzCenterPaperPartsUrlNew = "http://member.chengkao365.com/mapi/classroom/versionm/center/getCenterPaperParts";

        /// <summary>
        /// 获取试卷下的所有题目
        /// </summary>
         //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionInfoUrlNew = "http://member.chengkao365.com/mapi/qzdownload/phone/question/getPaperQuestionInfos";

        /// <summary>
        /// 根据试卷ID获取其下的所有选项
        /// </summary>
        //新接口 dgh 2018.01.15
        internal const string GetQzPaperQuestionOptionsUrlNew = "http://member.chengkao365.com/mapi/qzdownload/phone/question/getPaperQuestionOptions";

		/// <summary>
		/// 获取课程讲义下载接口
		/// </summary>
		//新接口dgh 2018.01.16
        internal const string GetSmallListKcjyUrlNew = "http://member.chengkao365.com/mapi/qzdownload/phone/cware/getSmallListKcjyWordUrl";

		/// <summary>
        /// 记录访问者
        /// </summary>
        internal const string RecordUserUrl = "http://mportal.zikao365.com/video/header/Visitor";
        
        /// <summary>
        /// 产生密钥
        /// </summary>
        internal const string GetGenKeyUrl = "http://mportal.zikao365.com/video/header/genkey";
        /// <summary>
        /// 获取文件头
        /// </summary>
        internal const string GetVideoHeadUrl = "http://mportal.zikao365.com/video/header/GetHead";

        /// <summary>
        /// 直接获取文件头
        /// </summary>
        internal const string GetVideoHeaderUrl = "http://mportal.zikao365.com/video/header/GetHeader";

		/// <summary>
		/// 获取课件视频对应的知识点测试及弹出时间
		/// </summary>
        //新接口 dgh 2018.01.17
        internal const string GetPointTestStartTimeUrlNew = "http://member.chengkao365.com/mapi/classroom/versionm/cware/getPointTestStartTime";
		
        /// <summary>
		/// 获取知识点测（练习）试题目信息
		/// </summary>
       //新接口 dgh 2018.01.17
        internal const string GetQuestionByPointTestIdUrlNew = "http://member.chengkao365.com/mapi/classroom/versionm/record/getQuestionByPointTestID";

		/// <summary>
		/// 保存知识点测试（练习）做题结果
		/// </summary>
         //新接口 dgh 2018.01.18
        internal const string SavePointTestQzResultUrlNew = "http://member.chengkao365.com/mapi/classroom/versionm/record/savePointTestQzResult";

		internal const string GetUserDataUrl = "http://member.chengkao365.com/newApi/classroom/member/getUserData.shtm";

        /// <summary>
        /// 下载协议学员开通课程信息  dgh 2015.06.23
        /// </summary>
        internal const string StudentGetOpenCourseDownloadUrl = "http://member.chengkao365.com/mapi/versionm/classroom/course/getOpenCourseDownload";
        
        /// <summary>
		/// 跳转到答疑板地址
		/// </summary>
		internal const string StudentFaqUrl = "http://member.chengkao365.com/member/loginDispose.shtm?sid={0}&gotoURL=http%3A%2F%2Fmember.chengkao365.com%2Ffaq%2FmyIndex.shtm";

         /// <summary>
        /// 播放/讲义的提问 dgh
        /// </summary>
        internal const string queryUrl = "http://member.chengkao365.com/faq/query.shtm?Forum_ID={0}&qclass=b0001&QNo={1}&Jy_url={2}";
        internal const string LoginQueryUrl = "http://mmember.chengkao365.com/member/loginDispose.shtm?gotoURL={0}";
         
        /// <summary>
        /// 恢复设备
        /// </summary>
       internal const string DeviceUrl="http://member.chengkao365.com/mobilewap/wap/bangdingpc/cancleEquipment.shtm";
         /// <summary>
        /// 保存做题记录
        /// </summary>
       internal const string SaveBatchMessageUrl = "http://member.chengkao365.com/mapi/classroom/versionm/record/saveBatchMessage";
        
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
       internal const string GetPaperScoreBatchUrl = "http://member.chengkao365.com/mapi/classroom/versionm/record/syncClouderPaperScoreBatch";
         
        /// <summary>
        /// 网页中选课
        /// </summary>
        internal const string GetSelectCourseUrl = "http://member.chengkao365.com//selcourse/index.shtm";
        
        /// <summary>
        /// 获取试卷提交次数 2017.08.11
        /// </summary>
        internal const string GetPaperSubmitCnt = "http://member.chengkao365.com/mapi/classroom/versionm/paper/getPaperSubmitCnt";
         
        /// <summary>
        /// 广告页
        /// </summary>
        internal const string AdvUrl = "http://member.chengkao365.com/mobilewap/wap/downloadClass/downloadClassTk.shtm";
        
        /// <summary>
        /// 我的服务
        /// </summary>
        internal const string MyServiceUrl = "http://member.chengkao365.com/mobilewap/wap/downloadClass/downloadClassInit.shtm";
       
        /// <summary>
        /// 同步有关视频的学员记录 dgh 2018.01.18
        /// </summary>
        internal const string SaveNextBeginTime = "http://member.chengkao365.com/mapi/versionm/classroom/course/saveNextBeginTime";
#endif

        #endregion

    }
}
