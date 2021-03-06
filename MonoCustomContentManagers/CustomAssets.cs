using System;
using System.Collections.Concurrent;
using CustomLoadMethod = System.Func<MonoCustomContentManagers.RefCountedContentManager, string, object>;
using CustomUnloadMethod = System.Action<MonoCustomContentManagers.RefCountedContentManager, object, string>;

namespace MonoCustomContentManagers
{
    /// <summary> Stores loading and unloading methods for custom types. </summary>
    public static class CustomAssets
    {
        private static readonly ConcurrentDictionary<Type, CustomLoadMethod> _CustomLoadMethods =
            new ConcurrentDictionary<Type, CustomLoadMethod>();

        private static readonly ConcurrentDictionary<Type, CustomUnloadMethod> _CustomUnloadMethods =
            new ConcurrentDictionary<Type, CustomUnloadMethod>();

        /// <summary> Registers the custom load method for given asset type. This method will be called
        ///  when it is time for an asset to load itself. </summary>
        /// <param name="assetType">Asset type.</param>
        /// <param name="assetLoad">Custom asset dispose method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assetType"/> is <see langword="null"/>
        /// -or-<paramref name="assetLoad"/> is <see langword="null"/></exception>
        public static void RegisterLoad(Type assetType, CustomLoadMethod assetLoad)
        {
            if (assetType == null) {
                throw new ArgumentNullException(nameof(assetType));
            }

            if (assetLoad == null) {
                throw new ArgumentNullException(nameof(assetLoad));
            }

            _CustomLoadMethods.AddOrUpdate(assetType, assetLoad, (_, __) => assetLoad);
        }

        /// <summary> Registers the custom load method for given asset type. This method will be called
        ///  when it is time for an asset to load itself. </summary>
        /// <typeparam name="T">Asset type.</typeparam>
        /// <param name="assetLoad">Custom asset dispose method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assetLoad"/> is <see langword="null"/></exception>
        public static void RegisterLoad<T>(CustomLoadMethod assetLoad)
        {
            if (assetLoad == null) {
                throw new ArgumentNullException(nameof(assetLoad));
            }

            RegisterLoad(typeof(T), assetLoad);
        }

        /// <summary> Registers the custom unload method for given asset type. This method will be called
        ///  when it is time for an asset to unload itself. </summary>
        /// <param name="assetType">Asset type.</param>
        /// <param name="assetUnload">Custom asset unload method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assetType"/> is <see langword="null"/>
        /// -or-<paramref name="assetUnload"/> is <see langword="null"/></exception>
        public static void RegisterUnload(Type assetType, CustomUnloadMethod assetUnload)
        {
            if (assetType == null) {
                throw new ArgumentNullException(nameof(assetType));
            }

            if (assetUnload == null) {
                throw new ArgumentNullException(nameof(assetUnload));
            }

            _CustomUnloadMethods.AddOrUpdate(assetType, assetUnload, (_, __) => assetUnload);
        }

        /// <summary> Registers the custom unload method for given asset type. This method will be called
        ///  when it is time for an asset to unload itself. </summary>
        /// <typeparam name="T">Asset type.</typeparam>
        /// <param name="assetUnload">Custom asset unload method.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assetUnload"/> is <see langword="null"/></exception>
        public static void RegisterUnload<T>(CustomUnloadMethod assetUnload)
        {
            if (assetUnload == null) {
                throw new ArgumentNullException(nameof(assetUnload));
            }

            RegisterUnload(typeof(T), assetUnload);
        }

        /// <summary> Finds previously registered custom load method for given asset type. </summary>
        /// <param name="assetType">Asset type for which load method was registered.</param>
        /// <returns>Found custom load method or null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assetType"/> is <see langword="null"/></exception>
        public static CustomLoadMethod FindLoad(Type assetType)
        {
            if (assetType == null) {
                throw new ArgumentNullException(nameof(assetType));
            }

            CustomLoadMethod customLoad;
            _CustomLoadMethods.TryGetValue(assetType, out customLoad);
            return customLoad;
        }

        /// <summary> Finds previously registered custom load method for given asset type. </summary>
        /// <typeparam name="T">Asset type for which load method was registered.</typeparam>
        /// <returns>Found custom load method or null.</returns>
        public static CustomLoadMethod FindLoad<T>()
        {
            return FindLoad(typeof(T));
        }

        /// <summary> Finds previously registered custom unload method for given asset type. </summary>
        /// <param name="assetType">Asset type for which unload method was registered.</param>
        /// <returns>Found custom disposal method or null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assetType"/> is <see langword="null"/></exception>
        public static CustomUnloadMethod FindUnload(Type assetType)
        {
            if (assetType == null) {
                throw new ArgumentNullException(nameof(assetType));
            }

            CustomUnloadMethod customDisposal;
            _CustomUnloadMethods.TryGetValue(assetType, out customDisposal);
            return customDisposal;
        }

        /// <summary> Finds previously registered custom unload method for given asset type. </summary>
        /// <typeparam name="T">Asset type for which unload method was registered.</typeparam>
        /// <returns>Found custom disposal method or null.</returns>
        public static CustomUnloadMethod FindUnload<T>()
        {
            return FindUnload(typeof(T));
        }

        /// <summary> Clears previous custom load method for given type. </summary>
        /// <param name="assetType">Asset type for which load method was registered.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assetType"/> is <see langword="null"/></exception>
        public static void ClearLoad(Type assetType)
        {
            if (assetType == null) {
                throw new ArgumentNullException(nameof(assetType));
            }

            CustomLoadMethod customLoad;
            _CustomLoadMethods.TryRemove(assetType, out customLoad);
        }

        /// <summary> Clears previous custom load method for given type. </summary>
        /// <typeparam name="T">Asset type for which load method was registered.</typeparam>
        public static void ClearLoad<T>()
        {
            ClearLoad(typeof(T));
        }

        /// <summary> Clears previous custom unload method for given type. </summary>
        /// <param name="assetType">Asset type for which unload method was registered.</param>
        public static void ClearUnload(Type assetType)
        {
            CustomUnloadMethod customDisposal;
            _CustomUnloadMethods.TryRemove(assetType, out customDisposal);
        }

        /// <summary> Clears previous custom unload method for given type. </summary>
        /// <typeparam name="T">Asset type for which unload method was registered.</typeparam>
        public static void ClearUnload<T>()
        {
            ClearUnload(typeof(T));
        }

        /// <summary> Removes \..\ from the path and changes path separator if needed. </summary>
        /// <param name="path">The path to clean.</param>
        /// <param name="directorySeparator">Platform directory separator.</param>
        /// <param name="altDirectorySeparator">Directory separator which will be replaced.</param>
        /// <returns></returns>
        internal static string CleanAssetPath(string path, char directorySeparator = '\\',
                                              char altDirectorySeparator = '/')
        {
            int pathEnd;
            path = path.Replace(altDirectorySeparator, directorySeparator);
            for (int i = 1; i < path.Length; i = Math.Max(pathEnd - 1, 1)) {
                i = path.IndexOf(@"\..\", i, StringComparison.OrdinalIgnoreCase);
                if (i < 0) {
                    return path;
                }

                pathEnd = path.LastIndexOf(directorySeparator, i - 1) + 1;
                path = path.Remove(pathEnd, (i - pathEnd) + @"\..\".Length);
            }

            return path;
        }
    }
}