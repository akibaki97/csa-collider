% Expanding-polytope algorithm in 2D
function [v,a,b] = epa(A,B,W,AW,BW)

    DEBUG__ = true;

    if DEBUG__,
        figure(1);     
        plot_polygon(A,'b');
        hold on;
        axis equal;
        plot_polygon(B,'r');

        figure(2);     
        CSO = cso(A,B);
        plot_polygon(CSO);
        hold on;
        axis equal;
        plot_polygon([W;W(1,:)]);
        hold on;
        plot(0,0,'kd');
    endif

    pq = pqueue(@(e1,e2) e1.d2 > e2.d2);
    m = rows(W);
    W = [W;W(1,:)];
    AW = [AW;AW(1,:)];
    BW = [BW;BW(1,:)];

    for i = 1:m,
        entry = construct_entry(W(i,:),W(i+1,:),AW(i,:),AW(i+1,:),BW(i,:),BW(i+1,:));

        figure(2);
        hold on;
        plot(entry.v(1),entry.v(2),'gx');

        if 0 < entry.t && entry.t < 1,
            pq.push(entry);
        endif      
    endfor

    if DEBUG__ && waitforbuttonpress != 0, DEBUG__ = false;
    endif

    while true,
        entry = pq.pop();
        v = entry.v;
        aw = support_mapping2d(A,v);
        bw = support_mapping2d(B,-v);
        w = aw - bw;

        if DEBUG__,
            figure(2);
            hold on;
            plot(v(1),v(2),'go');
            plot(w(1),w(2),'ro');
        endif

        % check if we're close enough to the pen. depth
        % i.e. lower and upper bound are sufficiently close to each other
        if dot(v,w) <= (1 + eps) * entry.d2,
            break;
        endif

        if DEBUG__,
            printf("dot(v,w)/norm(v) - norm(v) = \n\n%s\n",disp(dot(v,w)/norm(v) - norm(v)));
        endif;

        entry1 = construct_entry(entry.y1,w,entry.p1,aw,entry.q1,bw);
        if entry1.t == inf, break;
        endif

        if 0 < entry1.t && entry1.t < 1,  pq.push(entry1);
            if DEBUG__,
                figure(2);
                hold on;
                plot([entry.y1(1),w(1)],[entry.y1(2),w(2)],'k--');
            endif
        endif

        entry2 = construct_entry(entry.y2,w,entry.p2,aw,entry.q2,bw);
        if entry2.t == inf, break;
        endif

        if 0 < entry2.t && entry2.t < 1,  pq.push(entry2);
            if DEBUG__,
                figure(2);
                hold on;
                plot([entry.y2(1),w(1)],[entry.y2(2),w(2)],'k--');
            endif
        endif

        if DEBUG__ && waitforbuttonpress != 0, DEBUG__ = false;
        endif

    endwhile

    a = entry.p1 + entry.t * (entry.p2 - entry.p1);
    b = entry.q1 + entry.t * (entry.q2 - entry.q1);
    printf("a - b = \n\n%s\n",disp(a-b));

    if DEBUG__,
        figure(1);
        hold on;
        plot([a(1),b(1)],[a(2),b(2)],'g');

        figure(2);
        hold on;
        plot(v(1),v(2),'g*');
    endif

endfunction

function entry = construct_entry(y1,y2,p1,p2,q1,q2)

    entry.y1 = y1; % p1 - q1
    entry.y2 = y2; % p2 - q2
    entry.p1 = p1;
    entry.p2 = p2;
    entry.q1 = q1;
    entry.q2 = q2;
    dy = y2 - y1;

    if dot(dy,dy) < eps, entry.t = inf 
    else entry.t = dot(-dy,y1)/dot(dy,dy);
    endif

    entry.v = y1 + entry.t * dy;
    entry.d2 = dot(entry.v,entry.v);

endfunction
