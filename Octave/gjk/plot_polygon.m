function [] = plot_polygon(V,c)
    hold on;
    if exist('c'),
        plot(V(:,1),V(:,2),'color',c);
    else
        plot(V(:,1),V(:,2));
    endif
    hold off;
end