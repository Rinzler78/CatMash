using System;
using System.Diagnostics;
using System.Linq;

namespace CatMash.DataStore.EF
{
    public static class PropertiesExt
    {
        public static bool SetProperty<T>(this CatMashDbContext dbContext, string name, T value, bool eraseIfExist = true)
        {
            lock (dbContext)
            {
                try
                {
                    Property property = dbContext.Properties.FirstOrDefault(arg => arg.Name == name && arg.Type == value.GetType().Name);

                    if (property == null)
                    {
                        property = new Property
                        {
                            Name = name,
                            Value = value.ToString()
                        };

                        dbContext.Properties.Add(property);
                    }

                    property.Type = value.GetType().Name;

                    if (eraseIfExist)
                        property.Value = value.ToString();

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} : {ex}");
                }
            }

            return false;
        }

        public static bool GetDateProperty(this CatMashDbContext dbContext, string name, out DateTime value)
        {
            lock (dbContext)
            {
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        var property = dbContext.Properties.FirstOrDefault(arg => arg.Name == name && arg.Type == typeof(DateTime).Name);
                        if (property != null)
                        {
                            return DateTime.TryParse(property.Value, out value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} : {ex}");
                }

                value = default(DateTime);
                return false;
            }
        }

        public static bool GetIntProperty(this CatMashDbContext dbContext, string name, out int value)
        {
            lock (dbContext)
            {
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        var property = dbContext.Properties.FirstOrDefault(arg => arg.Name == name && arg.Type == typeof(Int32).Name);
                        if (property != null)
                        {
                            return Int32.TryParse(property.Value, out value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} : {ex}");
                }

                value = default(Int32);
                return false;
            }
        }

        public static bool GetBooleanProperty(this CatMashDbContext dbContext, string name, out bool value)
        {
            lock (dbContext)
            {
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        var property = dbContext.Properties.FirstOrDefault(arg => arg.Name == name && arg.Type == typeof(Boolean).Name);
                        if (property != null)
                        {
                            return Boolean.TryParse(property.Value, out value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} : {ex}");
                }

                value = default(Boolean);
                return false;
            }
        }

        public static bool GetProperty(this CatMashDbContext dbContext, string name, out string value)
        {
            lock (dbContext)
            {
                try
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        var property = dbContext.Properties.FirstOrDefault(arg => arg.Name == name && arg.Type == typeof(String).Name);
                        if (property != null)
                        {
                            value = property.Value;
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{System.Reflection.MethodBase.GetCurrentMethod().Name} : {ex}");
                }

                value = null;
                return false;
            }
        }
    }
}
