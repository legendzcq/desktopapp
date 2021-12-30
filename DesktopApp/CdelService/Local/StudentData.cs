using CdelService.Model;
using CdelService.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CdelService.Local
{
	public class StudentData : DataAccessBase
	{
		private const string GetStudentSql = "Select UserName,Password,SSOUID,LastLogin from Studentinfo";

		/// <summary>
		/// 获取当前已登录的用户
		/// </summary>
		/// <returns></returns>
		public StudentInfo GetLoginedStudent()
		{
			var dt = ExecuteTable(GetStudentSql);
			if (dt != null && dt.Rows.Count > 0)
			{
				var rw = dt.Rows[0];
				var item = new StudentInfo
				{
					UserName = rw.Field<string>("UserName"),
					Password = Crypt.Rc4DecryptString(rw.Field<string>("Password")).Trim(),
					Ssouid = rw.Field<int>("SSOUID"),
					LastLogin = rw.Field<DateTime>("LastLogin")
				};
				return item;
			}
			return null;
		}
	}
}
