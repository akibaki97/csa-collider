function [] = PlotVector3(v,col)
    tt = 0:1/100:1;
    hold on;
    if(col == 0)
        plot3(tt*v(1),tt*v(2),tt*v(3),'r');
        plot3(v(1),v(2),v(3),'r*');
    elseif(col == 1)
        plot3(tt*v(1),tt*v(2),tt*v(3),'b');
        plot3(v(1),v(2),v(3),'b*');
    elseif(col == 2)
        plot3(tt*v(1),tt*v(2),tt*v(3),'g');
        plot3(v(1),v(2),v(3),'g*');
    end
    
end