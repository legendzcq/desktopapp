using System;
using System.Windows;
using System.Windows.Controls;

namespace DesktopApp.Pages
{
	/// <summary>
	/// Interaction logic for AboutPage.xaml
	/// </summary>
	public partial class AboutPage : Page
	{
		public AboutPage()
		{
			InitializeComponent();
			InitInfo();
		}

		private void InitInfo()
		{
#if CHINAACC
			TxtTitle.Text = "中华会计网校—会计下载课堂";
			TxtCopyright.Text = "Copyright ©2000-" + DateTime.Now.Year + " www.chinaacc.com";
			TxtDes.Text = "　　中华会计网校（www.chinaacc.com）是权威、专业的大型会计远程教育基地，是联合国教科文组织技术与职业教育培训试点项目，凭借雄厚的师资力量、领先的课件技术、严谨的教学作风、灵活多样的学习方式、良好的教学效果，为我国财政系统培养了数百万名优秀的专业人才，被广大会计人员亲切地誉为“会计人的网上家园”！";
#endif
#if MED
            TxtTitle.Text = "医学下载课堂";
            TxtCopyright.Text = "Copyright ©2005-" + DateTime.Now.Year + " www.med66.com";
            TxtDes.Text = "　　医学教育网（www.med66.com）是大型医学远程教育网站，拥有医学专业信息及考试信息近百万条，注册学员数百万人，凭借雄厚的师资力量、领先的智能交互课件、严谨的教学作风、灵活多样的教学方式，受到了广大考生的一致好评，是考生了解医师资格、护士资格、卫生资格、执业药师等医学类考试最新政策、动态和参加培训的首选网站。";
#endif
#if JIANSHE
            TxtTitle.Text = "建筑下载课堂";
            TxtCopyright.Text = "Copyright ©2005-" + DateTime.Now.Year + " www.jianshe99.com";
            TxtDes.Text = "　　建设工程教育网（www.jianshe99.com）是大型建设工程类考试远程教育门户网站，主要从事一级/二级建造师、造价工程师等建工类考试网上辅导，并提供相关信息，是广大考生了解工程类考试最新政策和动态的首选网站，是广大建设工作者学习、工作的乐园！";
#endif
#if LAW
            TxtTitle.Text = "法律下载课堂";
            TxtCopyright.Text = "Copyright ©2003-" + DateTime.Now.Year + " www.chinalawedu.com";
            TxtDes.Text = "　　法律教育网（www.chinalawedu.com）是大型法律远程教育基地，拥有各类信息50余万条，注册学员近百万人，常年开设国家司法考试、法律实务课程等网上辅导，集结大量国内权威师资，以便捷的移动课堂、严谨的教学作风、良好的辅导效果，培养了大量优秀法律人才，在考生中有着极强的影响力和号召力。";
#endif
#if CHINATAT
            TxtTitle.Text = "职教下载课堂";
            TxtCopyright.Text = "Copyright ©2006-" + DateTime.Now.Year + " www.chinatat.com";
            TxtDes.Text = "　　职业培训教育网（www.chinatat.com）是集信息和培训于一体的职业类考试门户网站，主要从事经济师、职称英语、职称计算机、公务员考试等辅导培训，考试资讯丰富，辅导效果显著，形成了独特的考试品牌，深受学员喜爱，为我国培养了大量的专业人才，获得了学员及业界的一致认可。";
#endif
#if G12E
            TxtTitle.Text = "中小学下载课堂";
            TxtCopyright.Text = "Copyright ©2006-" + DateTime.Now.Year + " www.g12e.com";
            TxtDes.Text = "　　中小学教育网（www.g12e.com）是学生学习人大附中、人大附小等名校名师视频课程的首选网站，提供学校同步、周总结以及针对中高考的相关网络辅导课程；同时，网校推出了由知名教授与金牌教练员主讲的数学竞赛课程，并提供在线答疑、练习、模拟考试等服务，凭借其良好的效果赢得了家长和学生的一致好评。\r\n       网校相继推出了高清课件、移动应用（移动课堂、中小学问答、中高考每日一练、元素周期表、小学数学思维训练）及使用智能电视点播等功能，使学习变得更加轻松、有趣。";
#endif
#if ZIKAO
            TxtTitle.Text = "自考下载课堂";
            TxtCopyright.Text = "Copyright ©2000-" + DateTime.Now.Year + " www.zikao365.com";
            TxtDes.Text = "　　自考365（www.zikao365.com）是领先的专注于自学考试的专业网站，以其权威、准确、及时的信息，富有特色的自考社区及卓越的自学考试辅导效果，连年被评为“中国十佳网络教育机构”。学员可以根据自身的情况，自由、灵活地选择所需辅导课程。";
#endif
#if CHENGKAO
            TxtTitle.Text = "成考下载课堂";
            TxtCopyright.Text = "Copyright ©2005-" + DateTime.Now.Year + " www.chengkao365.com";
            TxtDes.Text = "　　成人高考教育网（www.chengkao365.com）是专门从事成人高考网上辅导的网站，考前辅导紧贴命题动向，直击考试精髓，是广大考生了解成人高考政策和动态的窗口，受到了百万成考生和求学者的一致好评和拥护。";
#endif
#if KAOYAN
            TxtTitle.Text = "考研下载课堂";
            TxtCopyright.Text = "Copyright ©2004-" + DateTime.Now.Year + " www.cnedu.cn";
            TxtDes.Text = "　　考研教育网（www.cnedu.cn）专注于全国硕士研究生入学统一考试的考前辅导培训，以优异的口碑、出众的效果以及高品质的服务获得了学员及业界的一致好评。";
#endif
#if FOR68
            TxtTitle.Text = "外语下载课堂";
            TxtCopyright.Text = "Copyright ©2005-" + DateTime.Now.Year + " www.for68.com";
            TxtDes.Text = "　　外语教育网（www.for68.com）是领先的外语远程教育基地，提供职称英语、学位英语、大学英语四/六级等多种英语类及多门小语种网上辅导课程，拥有各类外语专业信息及考试信息数十万条，是广大学员了解考试动态、参加语言培训的首选网站。";
#endif
		}

		private void btnSearch_Click(object sender, RoutedEventArgs e)
		{
			//string host = this.txtHostName.Text;
			//string ips = string.Join(",", Framework.Utility.Network.GetHostIpByDefaultDnsServer(host));
			//MessageBox.Show(ips);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			//string host = this.txtHostName.Text;
			//string ips = string.Join(",", Framework.Utility.Network.GetHostIpByOfficalDnsServer(host));
			//MessageBox.Show(ips);
		}
	}
}
