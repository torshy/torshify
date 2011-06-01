using System;

namespace Torshify.Core.Native
{
    internal class NativeSearchLink : NativeLink, ILink<ISearch>
    {
        #region Fields

        private ISearch _search;

        #endregion Fields

        #region Constructors

        public NativeSearchLink(ISession session, IntPtr handle, ISearch search)
            : base(session, handle)
        {
            _search = search;
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _search; }
        }

        ISearch ILink<ISearch>.Object
        {
            get { return (ISearch)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
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