using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace HydroR
{
    class SyntaxColoring
    {
        RichTextBox formattedText;
        syntax[] highlighters = new syntax[6];
        public RichTextBox getFormattedText()
        {
            return formattedText;
        }

        public SyntaxColoring()
        {
            highlighters[0] = new syntax(@"(\n|.)*", Color.Black);
            highlighters[1] = new syntax(@"(?<=\b)\d+(?=\b)", Color.DarkOrange);
            highlighters[2] = new syntax("(?<=\\b)(as\\.data\\.frame\\.model\\.matrix|getCConverterDescriptions|as\\.data\\.frame\\.numeric|as\\.data\\.frame\\.ordered|as\\.data\\.frame\\.POSIXct|as\\.data\\.frame\\.POSIXlt|model\\.matrix\\.glm\\.null|print\\.summary\\.aovlist|summary\\.packageStatus|USPersonalExpenditure|as\\.character\\.octmode|as\\.data\\.frame\\.logLik|as\\.data\\.frame\\.matrix|as\\.data\\.frame\\.vector|getTaskCallbackNames|makepredictcall\\.poly|package\\.dependencies|print\\.summary\\.manova|update\\.packageStatus|\\.PostScript\\.Options|as\\.character\\.POSIXt|as\\.data\\.frame\\.table|closeAllConnections|getCConverterStatus|getNativeSymbolInfo|package\\.description|print\\.packageStatus|print\\.summary\\.table|setCConverterStatus|taskCallbackManager|\\.NotYetImplemented|expand\\.model\\.frame|influence\\.measures|installed\\.packages|make\\.packages\\.html|print\\.recordedplot|removeTaskCallback|Summary\\.data\\.frame|summary\\.connection|summary\\.data\\.frame|weighted\\.residuals|xpdrows\\.data\\.frame|\\.BaseNamespaceEnv|all\\.equal\\.POSIXct|as\\.matrix\\.noquote|as\\.matrix\\.POSIXlt|download\\.packages|fitted\\.values\\.glm|format\\.data\\.frame|getAllConnections|getNumCConverters|NotYetImplemented|print\\.packageInfo|print\\.simple\\.list|print\\.summary\\.aov|print\\.summary\\.glm|as\\.data\\.frame\\.ts|coefficients\\.glm|fitted\\.values\\.lm|install\\.packages|interaction\\.plot|inverse\\.gaussian|LifeCycleSavings|make\\.search\\.html|package\\.contents|package\\.skeleton|print\\.connection|print\\.data\\.frame|print\\.dummy\\.coef|print\\.libraryIQR|print\\.packageIQR|print\\.SavedPlots|print\\.summary\\.lm|print\\.tables\\.aov|removeCConverter|socketConnection|zip\\.file\\.extract|addTaskCallback|as\\.table\\.ftable|boxplot\\.formula|coefficients\\.lm|contr\\.treatment|DateTimeClasses|delete\\.response|dev\\.interactive|edit\\.data\\.frame|lines\\.histogram|makepredictcall|model\\.matrix\\.lm|plot\\.data\\.frame|print\\.integrate|remove\\.packages|Summary\\.POSIXct|Summary\\.POSIXlt|showConnections|summary\\.aovlist|summary\\.default|summary\\.POSIXct|summary\\.POSIXlt|update\\.packages|winDialogString|\\.\\.path\\.package|as\\.environment|attr\\.all\\.equal|compareVersion|complete\\.cases|cooks\\.distance|filled\\.contour|format\\.default|format\\.octmode|format\\.POSIXct|format\\.POSIXlt|ftable\\.formula|Hypergeometric|is\\.empty\\.model|is\\.environment|link\\.html\\.help|memory\\.profile|model\\.response|nclass\\.Sturges|plot\\.histogram|points\\.formula|print\\.difftime|print\\.TukeyHSD|pushBackLength|round\\.difftime|SafePrediction|Summary\\.factor|Sys\\.localeconv|summary\\.factor|summary\\.manova|summary\\.matrix|sys\\.load\\.image|sys\\.save\\.image|terrain\\.colors|textConnection|USJudgeRatings|update\\.formula|variable\\.names|winMenuAddItem|winMenuDelItem|\\.find\\.package|\\.leap\\.seconds|\\.\\.primUntrace|anova\\.glmlist|as\\.data\\.frame|as\\.expression|boxplot\\.stats|CRAN\\.packages|check\\.options|contr\\.helmert|download\\.file|flush\\.console|format\\.factor|formula\\.terms|getConnection|geterrmessage|is\\.data\\.frame|is\\.expression|is\\.na\\.POSIXlt|library\\.dynam|limitedLabels|lines\\.formula|model\\.extract|model\\.weights|newestVersion|OrchardSprays|packageStatus|pairs\\.formula|plot\\.function|plot\\.TukeyHSD|print\\.aovlist|print\\.coefmat|print\\.default|print\\.density|print\\.formula|print\\.hsearch|print\\.noquote|print\\.octmode|print\\.ordered|print\\.POSIXct|print\\.POSIXlt|quasibinomial|Renviron\\.site|reg\\.finalizer|residuals\\.glm|Sys\\.getlocale|Sys\\.setlocale|summary\\.table|sunflowerplot|terms\\.formula|UCBAdmissions|weighted\\.mean|\\.\\.Autoloaded|\\.AutoloadEnv|\\.Random\\.seed|anova\\.lmlist|anovalist\\.lm|as\\.character|axis\\.POSIXct|capabilities|close\\.screen|close\\.socket|co\\.intervals|coefficients|contributors|count\\.fields|dev\\.copy2eps|edit\\.default|erase\\.screen|Formaldehyde|factor\\.scope|findInterval|forwardsolve|fourfoldplot|graphics\\.off|HairEyeColor|InsectSprays|index\\.search|is\\.character|is\\.recursive|isIncomplete|lm\\.influence|Math\\.POSIXlt|margin\\.table|mean\\.POSIXct|mean\\.POSIXlt|memory\\.limit|model\\.matrix|model\\.offset|model\\.tables|nclass\\.scott|old\\-piechart|old\\.packages|panel\\.smooth|parent\\.frame|plot\\.default|plot\\.density|plot\\.formula|plot\\.POSIXct|plot\\.POSIXlt|predict\\.poly|print\\.atomic|print\\.factor|print\\.family|print\\.ftable|print\\.listof|print\\.logLik|print\\.matrix|print\\.mtable|print\\.socket|quasipoisson|read\\.00Index|replications|residuals\\.lm|round\\.POSIXt|Sys\\.timezone|split\\.screen|storage\\.mode|summary\\.infl|sys\\.function|terms\\.object|trunc\\.POSIXt|type\\.convert|win\\.metafile|write\\.ftable|write\\.socket|\\[\\.data\\.frame|\\[\\.SavedPlots|\\.Deprecated|\\.Last\\.value|\\.NotYetUsed|\\.\\.Traceback|as\\.function|as\\.pairlist|char\\.expand|commandArgs|connections|contrib\\.url|DLL\\.version|data\\.matrix|dev\\.control|df\\.residual|discoveries|dump\\.frames|Exponential|edit\\.matrix|eff\\.aovlist|environment|expand\\.grid|file\\.access|file\\.append|file\\.choose|file\\.create|file\\.exists|file\\.remove|file\\.rename|format\\.char|format\\.info|format\\.pval|glm\\.control|heat\\.colors|help\\.search|hist\\.POSIXt|ISOdatetime|interaction|interactive|inverse\\.rle|is\\.function|is\\.infinite|is\\.language|is\\.pairlist|is\\.unsorted|loadhistory|Math\\.POSIXt|mahalanobis|make\\.socket|memory\\.size|model\\.frame|NegBinomial|Ops\\.POSIXct|Ops\\.POSIXlt|object\\.size|PlantGrowth|path\\.expand|plot\\.factor|plot\\.window|predict\\.glm|predict\\.mlm|print\\.anova|print\\.htest|print\\.table|print\\.terms|print\\.xtabs|Random\\.user|read\\.delim2|read\\.ftable|read\\.socket|reformulate|savehistory|se\\.contrast|searchpaths|select\\.list|summary\\.aov|summary\\.glm|summary\\.mlm|sys\\.on\\.exit|sys\\.parents|system\\.file|system\\.time|ToothGrowth|terms\\.terms|topo\\.colors|weights\\.glm|win\\.version|write\\.table|\\.First\\.lib|\\.GlobalEnv|\\.\\.libPaths|\\.Primitive|\\.primTrace|\\.ps\\.prolog|AIC\\.logLik|Arithmetic|abbreviate|airquality|as\\.complex|as\\.formula|as\\.integer|as\\.logical|as\\.numeric|as\\.ordered|as\\.POSIXct|as\\.POSIXlt|attributes|autoloader|bringToTop|bug\\.report|Comparison|case\\.names|connection|contr\\.poly|copyrights|cut\\.POSIXt|Deprecated|data\\.class|data\\.entry|data\\.frame|dev2bitmap|dir\\.create|drop\\.scope|drop\\.terms|dummy\\.coef|duplicated|dyn\\.unload|expression|extractAIC|family\\.glm|formula\\.lm|help\\.start|is\\.complex|is\\.element|is\\.integer|is\\.logical|is\\.numeric|is\\.ordered|isSeekable|list\\.files|localeconv|logLik\\.glm|make\\.names|mat\\.or\\.vec|match\\.call|mem\\.limits|month\\.name|mosaicplot|NextMethod|NotYetUsed|na\\.exclude|na\\.omit\\.ts|native\\.enc|Ops\\.POSIXt|parent\\.env|pentagamma|plot\\.table|pos\\.to\\.env|postscript|predict\\.lm|presidents|print\\.AsIs|print\\.infl|prop\\.table|ps\\.options|read\\.delim|read\\.table|recordPlot|replayPlot|Sys\\.getenv|Sys\\.putenv|seq\\.POSIXt|shell\\.exec|stack\\.loss|stat\\.anova|str\\.logLik|str\\.POSIXt|stripchart|substitute|summary\\.lm|symbol\\.For|sys\\.frames|sys\\.nframe|sys\\.parent|sys\\.source|sys\\.status|tetragamma|warpbreaks|weights\\.lm|winMenuAdd|winMenuDel|writeLines|xyz\\.coords|zip\\.unpack|\\[\\[\\.POSIXct|\\-\\.POSIXct|\\-\\.POSIXlt|\\.Dyn\\.libs|\\.External|\\.\\.Fortran|\\.Internal|\\.Last\\.lib|\\.\\.Options|\\.packages|\\.Platform|\\.Renviron|\\.Rprofile|add\\.scope|aggregate|all\\.equal|all\\.names|anova\\.glm|anova\\.mlm|approxfun|as\\.double|as\\.factor|as\\.matrix|as\\.single|as\\.symbol|as\\.vector|assocplot|backsolve|Chisquare|c\\.POSIXct|c\\.POSIXlt|character|charmatch|cm\\.colors|conflicts|contr\\.sum|contrasts|copyright|crossprod|dataentry|dev\\.print|dsignrank|family\\.lm|file\\.copy|file\\.info|file\\.path|file\\.show|frequency|GammaDist|Geometric|gammaCody|gctorture|getOption|globalenv|ISOLatin1|identical|integrate|intersect|invisible|is\\.atomic|is\\.double|is\\.factor|is\\.finite|is\\.loaded|is\\.matrix|is\\.object|is\\.single|is\\.symbol|is\\.vector|kronecker|Lognormal|logLik\\.lm|lower\\.tri|make\\.link|match\\.arg|match\\.fun|matpoints|month\\.abb|na\\.action|napredict|nclass\\.FD|pbirthday|prettyNum|print\\.aov|print\\.glm|print\\.rle|proc\\.time|psignrank|qbirthday|qsignrank|R\\.Version|R\\.version|read\\.csv2|readLines|residuals|row\\.names|rsignrank|rstandard|Subscript|Sys\\.sleep|sort\\.list|splinefun|stackloss|stopifnot|strheight|structure|substring|sys\\.calls|sys\\.frame|traceback|transform|USArrests|UseMethod|unix\\.time|upper\\.tri|which\\.max|which\\.min|win\\.graph|win\\.print|winDialog|write\\.dcf|writeChar|xy\\.coords|\\[\\.formula|\\[\\.noquote|\\[\\.POSIXct|\\[\\.POSIXlt|\\+\\.POSIXt|\\-\\.POSIXt|\\.\\.Device|\\.Devices|\\.Generic|\\.lib\\.loc|\\.Library|\\.Machine|airmiles|all\\.vars|anova\\.lm|anscombe|as\\.array|as\\.table|attitude|autoload|Binomial|basename|binomial|builtins|casefold|cbind\\.ts|chickwts|chol2inv|colMeans|colnames|convolve|covratio|debugger|dev\\.copy|dev\\.list|dev\\.next|dev\\.prev|deviance|difftime|dimnames|dotchart|dweibull|dyn\\.load|eurodist|faithful|formatDL|function|gaussian|identify|inherits|is\\.array|is\\.table|Japanese|La\\.eigen|Logistic|lines\\.ts|ls\\.print|MacRoman|matlines|optimise|optimize|Platform|p\\.adjust|pairlist|piechart|plot\\.gam|plot\\.mlm|plot\\.mts|plot\\.new|plotmath|polyroot|pressure|print\\.by|print\\.lm|print\\.ts|prmatrix|pushBack|pweibull|qr\\.resid|qr\\.solve|quantile|quarters|qweibull|Rconsole|Rprofile|read\\.csv|read\\.dcf|read\\.fwf|readChar|readline|rowMeans|rownames|rstudent|rweibull|SignRank|Sys\\.info|Sys\\.time|savePlot|segments|sequence|set\\.seed|setequal|strftime|strptime|strsplit|strwidth|sunspots|symbol\\.C|sys\\.call|TukeyHSD|tabulate|tempfile|termplot|toString|trigamma|truncate|url\\.show|VADeaths|Wilcoxon|warnings|weekdays|writeBin|zapsmall|\\[\\.factor|\\.\\.Group|\\.Method|\\.Script|apropos|as\\.call|as\\.list|as\\.name|as\\.null|as\\.real|barplot|besselI|besselJ|besselK|besselY|boxplot|browser|bw\\.nrd0|Control|ceiling|col2rgb|colours|colSums|comment|complex|contour|cumprod|Devices|dcauchy|density|deparse|dev\\.cur|dev\\.off|dev\\.set|dfbetas|diff\\.ts|digamma|dirname|dnbinom|do\\.call|dwilcox|Extract|effects|example|Foreign|fivenum|formals|formatC|formula|gc\\.time|glm\\.fit|Hershey|history|INSTALL|ISOdate|integer|is\\.call|is\\.list|is\\.name|is\\.null|is\\.real|islands|LETTERS|lchoose|letters|library|licence|license|lm\\.wfit|locales|locator|logical|longley|ls\\.diag|lsf\\.str|Machine|Methods|machine|matmult|matplot|max\\.col|methods|missing|n2mfrow|na\\.fail|na\\.omit|na\\.pass|naprint|naresid|new\\.env|nlevels|noquote|numeric|objects|on\\.exit|options|ordered|Poisson|POSIXct|POSIXlt|palette|pcauchy|plot\\.lm|plot\\.ts|plot\\.xy|pnbinom|poisson|polygon|ppoints|predict|preplot|profile|pwilcox|qcauchy|qnbinom|qwilcox|Rdindex|RNGkind|rainbow|rcauchy|readBin|recover|regexpr|relevel|replace|require|reshape|restart|rnbinom|rowSums|rwilcox|Special|Startup|Summary|setdiff|sprintf|stack\\.x|strwrap|summary|symbols|Titanic|tolower|toupper|Uniform|unclass|undebug|uniroot|unsplit|unstack|untrace|upgrade|version|volcano|Weibull|WinAnsi|warning|weights|windows|\\.Class|\\.First|abline|append|approx|arrows|assign|attach|attenu|Bessel|bessel|bitmap|bw\\.bcv|bw\\.nrd|bw\\.ucv|bzfile|Cauchy|chartr|choose|colors|coplot|cov\\.wt|cummax|cummin|cumsum|dbinom|dchisq|deltat|deriv3|detach|device|dffits|dgamma|dhyper|dlnorm|dlogis|double|exists|factor|family|fitted|format|freeny|ftable|gcinfo|gzfile|hasTsp|ifelse|infert|is\\.mts|is\\.nan|isOpen|jitter|julian|La\\.svd|labels|lapply|layout|legend|length|levels|lgamma|lm\\.fit|logLik|loglin|lowess|ls\\.str|Memory|manova|matrix|median|months|morley|mtcars|Normal|nhtemp|Ops\\.ts|offset|pbinom|pchisq|pgamma|phones|phyper|pictex|plnorm|plogis|pmatch|points|precip|pretty|prompt|ptukey|qbinom|qchisq|qgamma|qhyper|qlnorm|qlogis|qqline|qqnorm|qqplot|qr\\.qty|qtukey|quakes|R\\.home|Rd2dvi|Rd2txt|Rdconv|Rdevga|Recall|REMOVE|R_LIBS|rbinom|rchisq|remove|repeat|return|rgamma|rhyper|rivers|rlnorm|rlogis|rowsum|Syntax|sample|sapply|screen|search|signif|single|source|spline|stderr|stdout|subset|substr|switch|symnum|system|tapply|typeof|unique|unlink|unlist|unname|update|vector|window|xemacs|xyinch|\\[\\.AsIs|\\.Call|\\.Last|\\.Pars|acosh|alias|alist|anova|aperm|apply|array|as\\.qr|as\\.ts|asinh|atan2|atanh|BATCH|break|build|bw\\.SJ|cbind|check|chull|class|close|codes|curve|cycle|dbeta|debug|delay|deriv|dgeom|dnorm|dpois|drop1|dunif|eigen|emacs|esoph|evalq|expm1|FALSE|FDist|files|floor|frame|Gamma|gamma|getwd|image|iris3|is\\.na|is\\.qr|is\\.ts|kappa|Logic|lbeta|lines|local|log10|log1p|lsfit|match|merge|mtext|mvfft|names|nargs|nchar|nextn|optim|order|outer|Paren|pairs|parse|paste|pbeta|persp|pgeom|pnorm|polym|power|ppois|print|punif|qbeta|qgeom|qnorm|qpois|qr\\.qy|quasi|qunif|quote|Rprof|randu|range|rbeta|rbind|resid|rgeom|rnorm|round|rpois|runif|Sd2Rd|SHLIB|scale|setwd|shell|sleep|solve|split|stack|stars|start|state|stdin|sweep|swiss|TDist|Tukey|table|terms|title|trace|trees|trunc|union|uspop|which|while|women|write|xedit|xinch|xtabs|yinch|acos|add1|args|asin|atan|attr|axis|Beta|beta|body|Conj|call|cars|chol|coef|cosh|data|date|demo|dexp|dget|diag|diff|dput|drop|dump|edit|else|euro|eval|fifo|file|find|gray|grep|grey|grid|gsub|help|hist|iris|is\\.R|jpeg|list|load|log2|logb|Math|mean|menu|mode|NCOL|NROW|NULL|name|ncol|next|nrow|open|page|pexp|pico|pipe|plot|pmax|pmin|poly|prod|proj|qexp|qr\\.Q|qr\\.R|qr\\.X|quit|rank|real|rect|rexp|save|scan|seek|sign|sinh|sink|sort|sqrt|stem|step|stop|Trig|TRUE|tanh|text|time|unix|with|xfig|\\[\\.ts|AIC|Arg|abs|all|any|aov|ave|bmp|box|bxp|cat|co2|col|cor|cos|cov|cut|det|dim|dir|end|exp|fft|fix|for|get|glm|hat|hsv|Inf|IQR|lcm|log|Mod|mad|max|min|NaN|nlm|Ops|par|pdf|pie|png|RNG|rep|rev|rgb|rle|row|rug|seq|sin|str|sub|sum|svd|tan|try|tsp|unz|url|var|X11|x11|xor|\\.C|by|cm|de|df|dt|gc|gl|Im|if|lm|ls|NA|pf|pi|pt|qf|qr|qt|Re|rf|rm|rt|sd|ts|vi|C|c|D|F|I|q|T|t)(?=\\b)" +
                "", Color.DarkCyan);
            highlighters[3] = new syntax(@"(?<=\b)(while|if|for|else|do|ifelse)(?=\b)", Color.Blue);
            highlighters[4] = new syntax("\"([^(\\\")\n]|\\\\.)*\"", Color.DarkRed);
            highlighters[5] = new syntax("#+.*", Color.Green);


        }

        public void TextChange(RichTextBox rtb, int line)
        {
            if (rtb.Text != "")
            {
                if (line > rtb.Text.Length)
                    line = rtb.Text.Length;
                int begin = 0;
                // add the length of the lines
                for (int i = 0; i <= line - 1; i++)
                {
                    begin += rtb.Lines[i].Length;
                    // don't forget to add one more for the Environment.NewLine
                    begin += 1;
                }
                highlighting(rtb, rtb.Lines[line], begin);
            }
        }

        public void TextChange(RichTextBox rtb)
        {
            if (rtb.Text != "")
            {
                highlighting(rtb, rtb.Text, 0);
            }
        }

        public void TextChanged(RichTextBox rtb, int startline, int endline)
        {
            if (rtb.Text != "")
            {

                if (startline > rtb.Text.Length)
                    startline = rtb.Text.Length;
                if (endline > rtb.Text.Length)
                    endline = rtb.Text.Length;
                int begin = 0;
                // add the length of the lines
                for (int i = 0; i <= startline - 1; i++)
                {
                    begin += rtb.Lines[i].Length;
                    // don't forget to add one more for the Environment.NewLine
                    begin += 1;
                }
                string text = "";
                for (int i = startline; i < endline; i++)
                {
                    text += rtb.Lines[i];
                    text += "\r";
                }
                highlighting(rtb, text, begin);
            }
        }

        void highlighting(RichTextBox rtb, string text, int startchar)
        {
            foreach (syntax syn in highlighters)
            {
                Match m = syn.regex.Match(text);
                //Match m = syn.regex.Match(rtb.Text);
                while (m.Value != "")
                {
                    rtb.SelectionStart = startchar + m.Index;
                    //rtb.SelectionStart = m.Index;
                    rtb.SelectionLength = m.Length;
                    rtb.SelectionColor = syn.color;
                    rtb.SelectionLength = 0;
                    m = m.NextMatch();
                }
            }
            formattedText = rtb;
        }
    }

    public class syntax
    {
        public Regex regex;
        public Color color;
        public syntax(string r, Color c)
        {
            regex = new Regex(r);

            color = c;
        }
    }
}
