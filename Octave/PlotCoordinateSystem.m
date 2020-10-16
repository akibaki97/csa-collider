function [] = PlotCoordinateSystem(dim,len)

hold on;
if(dim == 3)
    PlotVector3([len,0,0],0);
    PlotVector3([0,len,0],2);
    PlotVector3([0,0,len],1);
else
    PlotVector2([len,0],0);
    PlotVector2([0,len],2);
end

end