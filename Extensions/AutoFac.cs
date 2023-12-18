using Autofac;

namespace exRate.Extensions
{
    namespace SharedUtils
    {
        /// <summary>
        /// Базовый класс для работы с зависимостями
        /// </summary>
        public static class AutoFac
        {
            private static ContainerBuilder _builder;
            private static ILifetimeScope _container;
            private static List<Type> _typeModules;

            /// <summary>
            /// Инициализация контейнера
            /// </summary>
            public static void Init(ContainerBuilder containerBuilder, Type[]? modules = null)
            {
                _builder = containerBuilder;
                _typeModules = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                from type in assembly.GetTypes()
                                where typeof(Module).IsAssignableFrom(type)
                                select type).ToList();

                if (modules != null)
                {
                    _typeModules.AddRange(modules);
                }

                foreach (var module in _typeModules)
                {
                    _builder.RegisterModule(Activator.CreateInstance(module) as Module);
                }

            }

            public static void SetContaionerAutoFac(ILifetimeScope container)
            {
                _container = container;
            }

            public static ILifetimeScope GetContainer()
            {
                return _container;
            }

            /// <summary>
            /// Получить сервис
            /// </summary>
            /// 
            /// <typeparam name="TService">Тип сервиса</typeparam>
            /// <param name="notException">Не выдавать исключение если не удалось получить объект По умолчанию false</param> 
            /// <returns>Экземпляр запрашиваемого сервиса</returns>
            public static TService Resolve<TService>(bool notException = false) where TService : class
            {
                if (!_container.IsRegistered<TService>() && notException)
                {
                    return null;
                }

                var service = _container.Resolve<TService>();
                return service;
            }


            public static ILifetimeScope CreateLifetimeScope()
            {
                return _container.BeginLifetimeScope();
            }

            /// <summary>
            /// Получить сервис по уникальному имени
            /// </summary>
            /// <typeparam name="TService">Экземпляр запрашиваемого сервиса</typeparam>
            /// <param name="serviceName">Уникальное имя запрашиваемого типа</param>
            /// <param name="notException">Не выдавать исключение если не удалось получить объект По умолчанию false</param>
            /// <returns></returns>
            public static TService ResolveNamedScoped<TService>(this ILifetimeScope scope, string serviceName,
                bool notException = false) where TService : class
            {
                if (!_container.IsRegisteredWithName<TService>(serviceName) && notException)
                {
                    return null;
                }

                var service = _container.ResolveNamed<TService>(serviceName);

                return service;
            }
        }
    }
}
