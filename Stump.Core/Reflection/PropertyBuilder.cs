using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Stump.Core.Reflection
{
    public static class PropertyBuilder
    {
        public static Type BuildDynamicTypeWithProperties(string classname, Dictionary<string, Type> properties)
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = Assembly.GetExecutingAssembly().GetName();
            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder myModBuilder = myAsmBuilder.DefineDynamicModule(myAsmName.Name);
            MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            TypeBuilder typebuilder = myModBuilder.DefineType(classname, TypeAttributes.Public);

            foreach (var propertie in properties)
            {
                FieldBuilder fieldbuilder = typebuilder.DefineField(propertie.Key.ToLower(), propertie.Value, FieldAttributes.Private);
                System.Reflection.Emit.PropertyBuilder getsetbuilder = typebuilder.DefineProperty(propertie.Key, PropertyAttributes.HasDefault, propertie.Value, null);

                //Define the "get" accessor method
                MethodBuilder getmethodbuilder = typebuilder.DefineMethod(string.Format("get_{0}", propertie.Key), getSetAttr, propertie.Value, Type.EmptyTypes);
                ILGenerator getil = getmethodbuilder.GetILGenerator();

                getil.Emit(OpCodes.Ldarg_0);
                getil.Emit(OpCodes.Ldfld, fieldbuilder);
                getil.Emit(OpCodes.Ret);

                //Define the "set" accessor method
                MethodBuilder setmethodbuilder = typebuilder.DefineMethod(string.Format("set_{0}", propertie.Key), getSetAttr, null, new Type[] { propertie.Value });
                ILGenerator setil = setmethodbuilder.GetILGenerator();

                setil.Emit(OpCodes.Ldarg_0);
                setil.Emit(OpCodes.Ldarg_1);
                setil.Emit(OpCodes.Stfld, fieldbuilder);
                setil.Emit(OpCodes.Ret);

                //Add get/set
                getsetbuilder.SetGetMethod(getmethodbuilder);
                getsetbuilder.SetSetMethod(setmethodbuilder);
            }

            return typebuilder.CreateType();
        }
    }
}
