using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace CodeSharp.EventSourcing.Castles
{
    /// <summary>
    /// WindsorContainer��չ
    /// </summary>
    public static class WindsorContainerExtensions
    {
        /// <summary>
        /// ע��һ��ָ�������ͼ���ӿ�
        /// </summary>
        public static IWindsorContainer RegisterType(this IWindsorContainer container, Type type)
        {
            //��������
            var life = ParseLife(type);
            //ʵ��ע��
            var typeKey = type.FullName;
            if (!container.Kernel.HasComponent(typeKey))
            {
                container.Register(Component.For(type).Named(typeKey).Life(life));
            }
            //�ӿ�ע��
            foreach (var interfaceType in type.GetInterfaces())
            {
                var key = interfaceType.FullName + "#" + type.FullName;
                if (!container.Kernel.HasComponent(key))
                {
                    container.Register(Component.For(interfaceType).ImplementedBy(type).Named(key).Life(life));
                }
            }

            return container;
        }
        /// <summary>
        /// ע����������Ķ�����ͼ���ӿ�
        /// </summary>
        public static IWindsorContainer RegisterTypes(this IWindsorContainer container, Func<Type, bool> typePredicate, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetExportedTypes().Where(x => typePredicate(x)))
                {
                    container.RegisterType(type);
                }
            }
            return container;
        }
        /// <summary>
        /// ������������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registration"></param>
        /// <param name="life"></param>
        /// <returns></returns>
        public static ComponentRegistration<T> Life<T>(this ComponentRegistration<T> registration, LifeStyle life) where T : class
        {
            if (life == LifeStyle.Singleton)
            {
                return registration.LifeStyle.Singleton;
            }
            return registration.LifeStyle.Transient;
        }

        private static LifeStyle ParseLife(Type type)
        {
            var componentAttributes = type.GetCustomAttributes(typeof(ComponentAttribute), false);
            return componentAttributes.Count() <= 0 ? LifeStyle.Transient : (componentAttributes[0] as ComponentAttribute).LifeStyle;
        }
    }
}