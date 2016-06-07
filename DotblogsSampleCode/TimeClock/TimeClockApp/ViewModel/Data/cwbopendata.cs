using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeClockApp
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:cwb:gov:tw:cwbcommon:0.1")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "urn:cwb:gov:tw:cwbcommon:0.1", IsNullable = false)]
    public partial class cwbopendata
    {

        private string identifierField;

        private string senderField;

        private System.DateTime sentField;

        private string statusField;

        private string msgTypeField;

        private string sourceField;

        private string dataidField;

        private string scopeField;

        private cwbopendataDataset datasetField;

        /// <remarks/>
        public string identifier
        {
            get
            {
                return this.identifierField;
            }
            set
            {
                this.identifierField = value;
            }
        }

        /// <remarks/>
        public string sender
        {
            get
            {
                return this.senderField;
            }
            set
            {
                this.senderField = value;
            }
        }

        /// <remarks/>
        public System.DateTime sent
        {
            get
            {
                return this.sentField;
            }
            set
            {
                this.sentField = value;
            }
        }

        /// <remarks/>
        public string status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string msgType
        {
            get
            {
                return this.msgTypeField;
            }
            set
            {
                this.msgTypeField = value;
            }
        }

        /// <remarks/>
        public string source
        {
            get
            {
                return this.sourceField;
            }
            set
            {
                this.sourceField = value;
            }
        }

        /// <remarks/>
        public string dataid
        {
            get
            {
                return this.dataidField;
            }
            set
            {
                this.dataidField = value;
            }
        }

        /// <remarks/>
        public string scope
        {
            get
            {
                return this.scopeField;
            }
            set
            {
                this.scopeField = value;
            }
        }

        /// <remarks/>
        public cwbopendataDataset dataset
        {
            get
            {
                return this.datasetField;
            }
            set
            {
                this.datasetField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:cwb:gov:tw:cwbcommon:0.1")]
    public partial class cwbopendataDataset
    {

        private cwbopendataDatasetDatasetInfo datasetInfoField;

        private cwbopendataDatasetLocation[] locationField;

        /// <remarks/>
        public cwbopendataDatasetDatasetInfo datasetInfo
        {
            get
            {
                return this.datasetInfoField;
            }
            set
            {
                this.datasetInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("location")]
        public cwbopendataDatasetLocation[] location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:cwb:gov:tw:cwbcommon:0.1")]
    public partial class cwbopendataDatasetDatasetInfo
    {

        private string datasetDescriptionField;

        private System.DateTime issueTimeField;

        private System.DateTime updateField;

        /// <remarks/>
        public string datasetDescription
        {
            get
            {
                return this.datasetDescriptionField;
            }
            set
            {
                this.datasetDescriptionField = value;
            }
        }

        /// <remarks/>
        public System.DateTime issueTime
        {
            get
            {
                return this.issueTimeField;
            }
            set
            {
                this.issueTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime update
        {
            get
            {
                return this.updateField;
            }
            set
            {
                this.updateField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:cwb:gov:tw:cwbcommon:0.1")]
    public partial class cwbopendataDatasetLocation
    {

        private string locationNameField;

        private cwbopendataDatasetLocationWeatherElement[] weatherElementField;

        /// <remarks/>
        public string locationName
        {
            get
            {
                return this.locationNameField;
            }
            set
            {
                this.locationNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("weatherElement")]
        public cwbopendataDatasetLocationWeatherElement[] weatherElement
        {
            get
            {
                return this.weatherElementField;
            }
            set
            {
                this.weatherElementField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:cwb:gov:tw:cwbcommon:0.1")]
    public partial class cwbopendataDatasetLocationWeatherElement
    {

        private string elementNameField;

        private cwbopendataDatasetLocationWeatherElementTime[] timeField;

        /// <remarks/>
        public string elementName
        {
            get
            {
                return this.elementNameField;
            }
            set
            {
                this.elementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("time")]
        public cwbopendataDatasetLocationWeatherElementTime[] time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:cwb:gov:tw:cwbcommon:0.1")]
    public partial class cwbopendataDatasetLocationWeatherElementTime
    {

        private System.DateTime startTimeField;

        private System.DateTime endTimeField;

        private cwbopendataDatasetLocationWeatherElementTimeParameter parameterField;

        /// <remarks/>
        public System.DateTime startTime
        {
            get
            {
                return this.startTimeField;
            }
            set
            {
                this.startTimeField = value;
            }
        }

        /// <remarks/>
        public System.DateTime endTime
        {
            get
            {
                return this.endTimeField;
            }
            set
            {
                this.endTimeField = value;
            }
        }

        /// <remarks/>
        public cwbopendataDatasetLocationWeatherElementTimeParameter parameter
        {
            get
            {
                return this.parameterField;
            }
            set
            {
                this.parameterField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "urn:cwb:gov:tw:cwbcommon:0.1")]
    public partial class cwbopendataDatasetLocationWeatherElementTimeParameter
    {

        private string parameterNameField;

        private string parameterUnitField;

        private byte parameterValueField;

        private bool parameterValueFieldSpecified;

        /// <remarks/>
        public string parameterName
        {
            get
            {
                return this.parameterNameField;
            }
            set
            {
                this.parameterNameField = value;
            }
        }

        /// <remarks/>
        public string parameterUnit
        {
            get
            {
                return this.parameterUnitField;
            }
            set
            {
                this.parameterUnitField = value;
            }
        }

        /// <remarks/>
        public byte parameterValue
        {
            get
            {
                return this.parameterValueField;
            }
            set
            {
                this.parameterValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool parameterValueSpecified
        {
            get
            {
                return this.parameterValueFieldSpecified;
            }
            set
            {
                this.parameterValueFieldSpecified = value;
            }
        }
    }


}
