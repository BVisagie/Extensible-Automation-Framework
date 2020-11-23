using EAF.Core.Base;

namespace EAF.Core.PageObjects
{
    public class SearchResultPage
    {
        private readonly Session sessionVariables;

        public SearchResultPage(Session sessionVariables)
        {
            this.sessionVariables = sessionVariables;
        }
    }
}