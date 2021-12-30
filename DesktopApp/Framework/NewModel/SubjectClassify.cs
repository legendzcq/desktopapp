using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
    [DataContract]
    public class SubjectClassifyReturnItem
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }
        
        //新接口使用
        [DataMember(Name = "msg")]
        public string Message { get; set; }

        [DataMember(Name = "subjectClassifyList")]
        public IEnumerable<SubjectClassify> ItemList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class SubjectClassify
    {
        [DataMember(Name = "subjectClassifyID")]
        public int SubjectClassifyId { get; set; }

        [DataMember(Name = "subjectName")]
        public string SubjectName { get; set; }

        [DataMember(Name = "treeType")]
        public int TreeType { get; set; }

        [DataMember(Name = "parentID")]
        public int ParentId { get; set; }
    }
    [DataContract]
    public class AccClassifyAnswer
    {
        [DataMember(Name = "Type")]
        public string Type { get; set; }
        [DataMember(Name = "Val")]
        public string Val { get; set; }
        [DataMember(Name = "Money")]
        public string Money { get; set; }
        [DataMember(Name = "ValText")]
        public string ValText { get; set; }
    }
}
