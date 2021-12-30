using Framework.Model;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
    /// <summary>
    /// 课程子项视图模型
    /// </summary>
    public class CourseDetailViewModel : ViewModelBase
    {
        public string CourseName { get; set; }
        public string CwareClassName { get; set; }
        public string CTeacherName { get; set; }
        public string CYearName { get; set; }
        public bool IsOpen { get; set; }
        public ViewStudentCourseWare Model { get; set; }

        public void FromModel(ViewStudentCourseWare model)
        {
            if (model == null)
                return;
            Model = model;
            CourseName = model.CourseName;
            CwareClassName = model.CWareClassName;
            CTeacherName = model.CTeacherName;
            IsOpen = model.IsOpen;
            CYearName = model.CYearName;
        }
    }
}
