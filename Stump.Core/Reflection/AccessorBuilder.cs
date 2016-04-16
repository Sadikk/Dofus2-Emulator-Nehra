using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Stump.Core.Reflection
{
    public class AccessorBuilder
    {
        public delegate object GetFieldValueBoundDelegate();

        public delegate object GetFieldValueUnboundDelegate(object instance);

        public delegate void SetFieldValueBoundDelegate(object value);

        public delegate void SetFieldValueUnboundDelegate(object instance, object value);

        private static DynamicMethod CreateGetterImpl(Type instanceType, Type fieldType, string fieldName)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (fieldName.Length == 0)
            {
                throw new ArgumentException("Field name must be non-empty string");
            }
            FieldInfo field = instanceType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new MissingFieldException("Can't obtain field " + fieldName + " from class " + instanceType.Name);
            }
            if (!fieldType.IsAssignableFrom(field.FieldType))
            {
                throw new InvalidCastException(string.Concat(new object[]
				{
					"Field ",
					fieldName,
					" of type ",
					field.FieldType,
					" can not be casted to ",
					fieldType.FullName
				}));
            }
            DynamicMethod dynamicMethod = new DynamicMethod(instanceType.Name + "_" + fieldName + "_Getter", fieldType, new Type[]
			{
				instanceType
			}, instanceType);
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0);
            iLGenerator.Emit(OpCodes.Ldfld, field);
            iLGenerator.Emit(OpCodes.Ret);
            return dynamicMethod;
        }

        private static DynamicMethod CreateSetterImpl(Type instanceType, Type fieldType, string fieldName)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (fieldName.Length == 0)
            {
                throw new ArgumentException("Field name must be non-empty string");
            }
            FieldInfo field = instanceType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new MissingFieldException("Can't obtain field " + fieldName + " from class " + instanceType.Name);
            }
            if (!fieldType.IsAssignableFrom(field.FieldType))
            {
                throw new InvalidCastException(string.Concat(new object[]
				{
					"Field ",
					fieldName,
					" of type ",
					field.FieldType,
					" can not be casted to ",
					fieldType.FullName
				}));
            }
            DynamicMethod dynamicMethod = new DynamicMethod(instanceType.Name + "_" + fieldName + "_Setter", typeof(void), new Type[]
			{
				typeof(object),
				typeof(object)
			}, instanceType);
            ILGenerator iLGenerator = dynamicMethod.GetILGenerator();
            LocalBuilder local = iLGenerator.DeclareLocal(field.DeclaringType);
            iLGenerator.Emit(OpCodes.Ldarg_0);
            if (field.DeclaringType.IsValueType)
            {
                iLGenerator.Emit(OpCodes.Unbox_Any, field.DeclaringType);
                iLGenerator.Emit(OpCodes.Stloc_0, local);
                iLGenerator.Emit(OpCodes.Ldloca_S, local);
            }
            else
            {
                iLGenerator.Emit(OpCodes.Castclass, field.DeclaringType);
                iLGenerator.Emit(OpCodes.Stloc_0, local);
                iLGenerator.Emit(OpCodes.Ldloc_0, local);
            }
            iLGenerator.Emit(OpCodes.Ldarg_1);
            iLGenerator.Emit(field.FieldType.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, field.FieldType);
            iLGenerator.Emit(OpCodes.Stfld, field);
            iLGenerator.Emit(OpCodes.Ret);
            return dynamicMethod;
        }

        public static AccessorBuilder.GetFieldValueUnboundDelegate CreateGetter(Type instanceType, Type fieldType, string fieldName)
        {
            DynamicMethod method = AccessorBuilder.CreateGetterImpl(instanceType, fieldType, fieldName);
            return (AccessorBuilder.GetFieldValueUnboundDelegate)method.CreateDelegate(new Type[]
			{
				typeof(AccessorBuilder.GetFieldValueUnboundDelegate)
			});
        }

        public static AccessorBuilder.GetFieldValueBoundDelegate CreateGetter(object instance, Type fieldType, string fieldName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            DynamicMethod dynamicMethod = AccessorBuilder.CreateGetterImpl(instance.GetType(), fieldType, fieldName);
            return (AccessorBuilder.GetFieldValueBoundDelegate)dynamicMethod.CreateDelegate(typeof(AccessorBuilder.GetFieldValueBoundDelegate), instance);
        }

        public static AccessorBuilder.SetFieldValueUnboundDelegate CreateSetter(Type instanceType, Type fieldType, string fieldName)
        {
            DynamicMethod method = AccessorBuilder.CreateSetterImpl(instanceType, fieldType, fieldName);
            return (AccessorBuilder.SetFieldValueUnboundDelegate)method.CreateDelegate(new Type[]
			{
				typeof(AccessorBuilder.SetFieldValueUnboundDelegate)
			});
        }

        public static AccessorBuilder.SetFieldValueBoundDelegate CreateSetter(object instance, Type fieldType, string fieldName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            DynamicMethod dynamicMethod = AccessorBuilder.CreateSetterImpl(instance.GetType(), fieldType, fieldName);
            return (AccessorBuilder.SetFieldValueBoundDelegate)dynamicMethod.CreateDelegate(typeof(AccessorBuilder.SetFieldValueBoundDelegate), instance);
        }
    }
}