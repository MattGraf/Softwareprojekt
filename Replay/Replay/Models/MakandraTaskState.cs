namespace Replay.Models
{
    /// <summary>
    /// Model for the different states a <see cref="MakandraTask"/>
    /// can be in.
    /// </summary>
    /// <author>Thomas Dworshak</author>
    public class MakandraTaskState
    {
        [Key]
        public int Id { get; set; }
        public string Name { set; get; }
    }
}