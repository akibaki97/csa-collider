% Implementation of the Minkowsky Portal Refinement algorithm in 2D
function [v,a,b,hit] = mpr(A,B)

    DEBUG__ = true;

    if DEBUG__,
        figure(1);
        axis equal;
        plot_polygon(A,'b');
        plot_polygon(B,'r');

        figure(2);
        axis equal;
        CSO = cso(A,B);
        plot_polygon(CSO,'k');
        hold on;
        plot(0,0,'k+');
        waitforbuttonpress;
    endif

    % --- Phase1: Portal discovery ---

    % - find origin ray -
    av0 = sum(A,1)/length(A);
    bv0 = sum(B,1)/length(B);
    v0 = av0 - bv0;

    % - find candidate portal -

    av1 = support_mapping2d(A,-v0);
    bv1 = support_mapping2d(B,v0);
    v1 = av1 - bv1;

    n = v1 - v0;
    n = [n(2),-n(1)];


    % choose the side where the origin lies
    if dot(n,-v0) < 0,
        n = -n;
    endif

    av2 = support_mapping2d(A,n);
    bv2 = support_mapping2d(B,-n);
    v2 = av2 - bv2;

    portal = [v1;v2];

    if DEBUG__,
        figure(2);
        hold on;
        plot(v0(1),v0(2),'ro');
        plot([-v0(1),v0(1)],[-v0(2),v0(2)],'k');

        waitforbuttonpress;

        plot([v0(1),v1(1)],[v0(2),v1(2)],'r--');
        waitforbuttonpress;
        plot([v0(1),v2(1)],[v0(2),v2(2)],'r--');
        waitforbuttonpress;
        plot([v1(1),v2(1)],[v1(2),v2(2)],'g--');

        waitforbuttonpress;
    endif


    % --- Phase 2: Portal refinement ---
    max_it = 20;
    it = 0;
    hit = false;

    while it++ < max_it,

        % check if origin is inside the portal
        n = v2 - v1;
        n = [n(2),-n(1)];       
        sv0 = dot(n,v0-v1);
        so = dot(n,-v1);
        
        if sv0 * so > 0,
            hit = true;

            % hit found
            if DEBUG__,
                printf("origin is inside the portal\n\n");
            endif

            % let n be the normal of the portal pointing out from the CSO
            n *= -sign(so);
        else
            % let n be the normal of the portal pointing out from the CSO
            n *= sign(so);
        endif

        % find support in direction of portal       
        av3 = support_mapping2d(A,n);
        bv3 = support_mapping2d(B,-n);
        v3 = av3 - bv3;

        if DEBUG__,
            hold on;   
            plot(v3(1),v3(2),'go');
        endif

        if dot(n,-v3) > 0,
            % origin is outside of the support plane
            if DEBUG__,
                printf("\norigin is outside of the support plane\n\n");
            endif
            break;
        endif

        if dot(n,v3-v1)/norm(n) < eps,
            % support plane is close to portal
            if DEBUG__,
                printf("\nsupport plane is close enough to portal\n\n");
            endif
            break;
        endif

        % choose new portal
        n = v3 - v0;
        n = [n(2),-n(1)];
        sv1 = dot(n,v1-v0);
        so = dot(n,-v0);
        if so * sv1 > 0,
            v2 = v3;
            av2 = av3;
            bv2 = bv3;
        else
            v1 = v3;
            av1 = av3;
            bv1 = bv3;
        endif

        portal = [v1;v2];

        if DEBUG__,
            figure(2);
            hold on;
            plot([v0(1),v1(1)],[v0(2),v1(2)],'r--');
            waitforbuttonpress;
            plot([v0(1),v2(1)],[v0(2),v2(2)],'r--');
            waitforbuttonpress;
            plot([v1(1),v2(1)],[v1(2),v2(2)],'g--');
        endif

    endwhile

    [v,lambda] = closest_pt(v1,v2);
    a = lambda * [av1;av2];
    b = lambda * [bv1;bv2];

    if DEBUG__,

        if hit,
            figure(1);
            hold on;
            plot([a(1),b(1)],[a(2),b(2)],'g');
            plot(a(1),a(2),'c*');
            plot(b(1),b(2),'m*');
            figure(2);
            hold on;
            plot(v(1),v(2),'b+','markersize',14);
            figure(3);
            A .-= v;
            plot_polygon(A,'b');
            hold on;
            plot_polygon(B,'r');
        endif

        printf("number of iterations: %d\n\n",it);
    endif
    
end

function [v,lambda] = closest_pt(v1,v2)

    dv = v2 - v1;
    d2v = dot(dv,dv);
    if d2v < eps,
        lambda = [Inf,Inf];
        v = Inf;
    endif

    lambda(2) = dot(-dv,v1)/d2v;
    lambda(1) = 1 - lambda(2);
    v = lambda * [v1;v2];

endfunction