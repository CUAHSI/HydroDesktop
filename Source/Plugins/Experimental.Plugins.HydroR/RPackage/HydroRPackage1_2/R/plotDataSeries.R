plotDataSeries <-
function(datavals, ylabel, ...)
{                                                        
    plot(datavals$DataValues$LocalDateTime, datavals$DataValues$DataValue, xlab="Date", ylab=ylabel, ...)
}

