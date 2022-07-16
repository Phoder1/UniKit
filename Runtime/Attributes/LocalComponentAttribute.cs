using UnityEngine;
namespace CustomAttributes
{
        public enum GetComponentTargets { Local, Childrens, Parents, Anywhere}
    public class LocalComponentAttribute : PropertyAttribute
    {
        public readonly GetComponentTargets getComponentFromChildrens = GetComponentTargets.Local;
        public readonly bool hideProperty = false;
        public readonly string parentObject = null;
        public readonly bool lockProperty = false;
        public readonly bool includeInactive = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hideProperty">
        /// If true the property will be hidden if assigned.
        /// </param>
        /// <param name="getComponentTargets">
        /// If true, will try to find the component in child objects.
        /// </param>
        /// <param name="parentObject">
        /// A string with a gameobject name can be assigned. will try to look for components in that gameobject.
        /// </param>
        /// <param name="lockProperty">
        /// If true the property will be unassignable manually. 
        /// </param>
        public LocalComponentAttribute(bool hideProperty = false, bool lockProperty = false, GetComponentTargets getComponentTargets = GetComponentTargets.Local, bool includeInactive = false, string parentObject = null)
        {
            this.getComponentFromChildrens = getComponentTargets;
            this.hideProperty = hideProperty;
            this.parentObject = parentObject;
            this.lockProperty = lockProperty;
            this.includeInactive = includeInactive;
        }
    }
}