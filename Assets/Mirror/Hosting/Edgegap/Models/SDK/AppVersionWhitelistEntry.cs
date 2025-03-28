using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace IO.Swagger.Model {

  /// <summary>
  /// 
  /// </summary>
  [DataContract]
  public class AppVersionWhitelistEntry {
    /// <summary>
    /// Unique ID of the entry
    /// </summary>
    /// <value>Unique ID of the entry</value>
    [DataMember(Name="id", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// CIDR to allow
    /// </summary>
    /// <value>CIDR to allow</value>
    [DataMember(Name="cidr", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "cidr")]
    public string Cidr { get; set; }

    /// <summary>
    /// Label to organized your entries
    /// </summary>
    /// <value>Label to organized your entries</value>
    [DataMember(Name="label", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "label")]
    public string Label { get; set; }

    /// <summary>
    /// If the Rule will be applied on runtime
    /// </summary>
    /// <value>If the Rule will be applied on runtime</value>
    [DataMember(Name="is_active", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "is_active")]
    public bool? IsActive { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      StringBuilder sb = new StringBuilder();
      sb.Append("class AppVersionWhitelistEntry {\n");
      sb.Append("  Id: ").Append(Id).Append("\n");
      sb.Append("  Cidr: ").Append(Cidr).Append("\n");
      sb.Append("  Label: ").Append(Label).Append("\n");
      sb.Append("  IsActive: ").Append(IsActive).Append("\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}
}
