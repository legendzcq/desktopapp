using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.Model
{
    [DataContract]
    public class PaperPartListReturn
    {
        [DataMember(Name = "success")]
        public string Success { get; set; }

        [DataMember(Name = "retry")]
        public string Retry { get; set; }

        [DataMember(Name = "result")]
        public PaperPartListResult Result { get; set; }
    }

    [DataContract]
    public class PaperPartListResult
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "paperPartsList")]
        public IEnumerable<StudentPaperPart> PaperPartsList { get; set; }
    }

    [DataContract]
    public class StudentPaperPart
    {
        [DataMember(Name = "partID")]
        public int PartId { get; set; }

        [DataMember(Name = "paperID")]
        public int PaperId { get; set; }

        [DataMember(Name = "partName")]
        public string PartName { get; set; }

        [DataMember(Name = "sequence")]
        public int Sequence { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [DataMember(Name = "creator")]
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember(Name = "createTime")]
        public string CreateTime { get; set; }

        [DataMember(Name="quesViewType")]
        public int QuesViewType { get; set; }

        [DataMember(Name = "randomNum")]
        public string RandomNum { get; set; }

        //新添加 dgh 2018.01.12
        [DataMember(Name = "quesTypeID")]
        public int quesTypeID { get; set; }
    }
}
