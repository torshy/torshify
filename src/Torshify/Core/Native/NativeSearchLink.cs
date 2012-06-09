using System;

namespace Torshify.Core.Native
{
    internal class NativeSearchLink : NativeLink, ILink<ISearch>
    {
        #region Fields

        private Lazy<ISearch> _search;
        private string _searchLink;

        #endregion Fields

        #region Constructors

        public NativeSearchLink(ISession session, IntPtr handle, string searchLink)
            : base(session, handle)
        {
            _searchLink = searchLink.Replace("spotify:search:", string.Empty);
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _search.Value; }
        }

        ISearch ILink<ISearch>.Object
        {
            get { return (ISearch)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _search = new Lazy<ISearch>(() =>
                                            {
                                                AssertHandle();

                                                lock (Spotify.Mutex)
                                                {
                                                    return Session.Search(_searchLink, 0, 250, 0, 250, 0, 250, 0, 250, SearchType.Standard);
                                                }
                                            });
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods
    }
}