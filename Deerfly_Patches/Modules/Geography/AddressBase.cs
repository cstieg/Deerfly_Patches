using System.Reflection;

namespace Deerfly_Patches.Modules.Geography
{
    public class AddressBase
    {
        public virtual string Recipient { get; set; }

        public virtual string Address1 { get; set; }

        public virtual string Address2 { get; set; }

        public virtual string City { get; set; }

        public virtual string State { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual string Country { get; set; }

        public virtual string Phone { get; set; }

        public virtual AddressType Type { get; set; }

        public override string ToString()
        {
            return Address1 + " " + Address2 + ", " + City + ", " + State + " " + PostalCode;
        }

        public bool AddressIsSame(AddressBase address)
        {
            return Address1.Trim() == address.Address1.Trim() &&
                Address2.Trim() == address.Address2.Trim() &&
                City.Trim() == address.City.Trim() &&
                State.Trim() == address.State.Trim() &&
                PostalCode.Trim() == address.PostalCode.Trim();
        }

        public bool PhoneIsSame(AddressBase address)
        {
            return Phone.Digits() == address.Phone.Digits();
        }

        public void CopyTo(AddressBase address)
        {
            PropertyInfo[] properties = GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo copyFromProperty = properties[i];
                PropertyInfo copyToProperty = address.GetType().GetProperty(copyFromProperty.Name);
                if (copyToProperty != null)
                {
                    copyToProperty.SetValue(address, copyFromProperty.GetValue(this));
                }

            }
        }

        public void SetNullStringsToEmpty()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].PropertyType == typeof(string) && properties[i].GetValue(this) == null)
                {
                    properties[i].SetValue(this, "");
                }
            }
        }
    }

    public enum AddressType
    {
        Billing,
        Shipping
    }
}