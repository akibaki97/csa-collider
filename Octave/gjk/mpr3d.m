% Implementation of the Minkowsky Portal Refinement algorithm in 3D
function [hit,v,a,b] = mpr3d(A,B,FA,FB,AdjA,AdjB)

    DEBUG__ = true;

    if DEBUG__,
        figure(1);
        axis equal;
        hold on;
        plot_polyhedron(A,FA,'b');
        plot_polyhedron(B,FB,'r');

        figure(2);
        axis equal;
        [CSO,FCSO] = cso3d(A,B);
        plot_polyhedron(CSO,FCSO,'k:');
        waitforbuttonpress;
    endif

    % --- Phase 1: Portal discovery ---

    % - find origin ray -
    aw0 = sum(A,1) / length(A);
    bw0 = sum(B,1) / length(B);
    
    % the interior point:
    w0 = aw0 - bw0;

    % - find candidate portal -
    aw1 = support_mapping3d(A,AdjA,-w0);
    bw1 = support_mapping3d(B,AdjB,w0);
    w1 = aw1 - bw1;

    n = cross(w1,w0);
    aw2 = support_mapping3d(A,AdjA,n);
    bw2 = support_mapping3d(B,AdjB,-n);
    w2 = aw2 - bw2;

    n = cross(w1 - w0, w2 - w0);
    aw3 = support_mapping3d(A,AdjA,n);
    bw3 = support_mapping3d(B,AdjB,-n);
    w3 = aw3 - bw3;

    W = [w1;w2;w3];
    AW = [aw1;aw2;aw3];
    BW = [bw1;bw2;bw3];

    if DEBUG__,
        figure(2);
        hold on;
        plot3(w0(1),w0(2),w0(3),'ro');
        plot3(0,0,0,'k+','markersize',12);
        plot3([-w0(1),w0(1)],[-w0(2),w0(2)],[-w0(3),w0(3)],'r');
        waitforbuttonpress;
    endif


    wrong = 0;
    max_it = 10;
    while max_it--,
        [wrong,n] = origin_vs_portal_candidate(w0,W);

        if !wrong, break;
        endif

        % - choose new candidate -
        AW(wrong,:) = support_mapping3d(A,AdjA,n);
        BW(wrong,:) = support_mapping3d(B,AdjB,-n);
        W(wrong,:) = AW(wrong,:) - BW(wrong,:);
    endwhile

    portal_plot = {[],[],[]};

    if DEBUG__,
        figure(2);
        hold on;
        next = [2,3,1];
        for i = 1:3,
            X = [w0(1),W(i,1),W(next(i),1),w0(1)];
            Y = [w0(2),W(i,2),W(next(i),2),w0(2)];
            Z = [w0(3),W(i,3),W(next(i),3),w0(3)];
            portal_plot{i} = plot3(X,Y,Z,'g');
        endfor

        waitforbuttonpress;
    endif

    % --- Phase 2: Portal refinement ---

    max_it = 20;
    it = 0;
    hit = false;
    tetra_plot = {[],[],[]};

    while it++ < max_it,

        % check if origin lies inside the portal
        n = cross(W(3,:)-W(1,:),W(2,:)-W(1,:));
        s0 = dot(n,-W(1,:));
        sw0 = dot(n,w0-W(1,:)); 

        if s0 * sw0 > 0,
            if DEBUG__ && !hit,
                printf("origin is inside the portal\n\n");
            endif
            
            hit = true;

            % let n be the normal of the portal pointing out from the CSO
            n *= -sign(s0);
        else
            % let n be the normal of the portal pointing out from the CSO
            n *= sign(s0);
        endif

        % find support in the direction of the portal normal
        aw4 = support_mapping3d(A,AdjA,n);
        bw4 = support_mapping3d(B,AdjB,-n);
        w4 = aw4 - bw4;

        if DEBUG__,
            figure(2);
            hold on;
            plot3(w4(1),w4(2),w4(3),'co');

            waitforbuttonpress;
            next = [2,3,1];
            for i = 1:3,
                set(tetra_plot{i},'visible','off');
                X = [w4(1),W(i,1),W(next(i),1),w4(1)];
                Y = [w4(2),W(i,2),W(next(i),2),w4(2)];
                Z = [w4(3),W(i,3),W(next(i),3),w4(3)];
                tetra_plot{i}=plot3(X,Y,Z,'b');
                set(portal_plot{i},'visible','on');
            endfor

            waitforbuttonpress;
            x = w4 + n/norm(n);
            plot3([w4(1),x(1)],[w4(2),x(2)],[w4(3),x(3)],'r--');
        endif

        % check if origin lies outside of the support plane
        if dot(n,-w4) > 0,
            if DEBUG__,
                printf("\norigin is outside of the support plane\n\n");
            endif
            break;
        endif

        % check if support plane is close enough to the portal
        dot(n,w4 - W(1,:))/norm(n)
        if dot(n,w4 - W(1,:))/norm(n) < eps,
            if DEBUG__,
                printf("\nsupport plane is close enough to portal\n\n");
            endif
            break;
        endif
        
        % choose new portal
        % if !hit,
        %     ind = new_portal_vertex_index(w0,W,w4);
        % else
        %     ind = new_portal_vertex_index(w4,W,w0);
        % endif
        if !hit,
            ind = new_portal_vertex_index(w0,W,w4,[0,0,0],false);
        else
            t = -dot(W(1,:),n)/dot(w0,n);
            plot3(-t*w0(1),-t*w0(2),-t*w0(3),'g*');
            ind = new_portal_vertex_index(w0,W,w4,-t*w0,true);
        endif


        W(ind,:) = w4;
        AW(ind,:) = aw4;
        BW(ind,:) = bw4;

        if DEBUG__,
            figure(2);
            hold on;
            next = [2,3,1];
            for i = 1:3,
                set(portal_plot{i},'visible','off');
                set(tetra_plot{i},'visible','off');
                X = [w0(1),W(i,1),W(next(i),1),w0(1)];
                Y = [w0(2),W(i,2),W(next(i),2),w0(2)];
                Z = [w0(3),W(i,3),W(next(i),3),w0(3)];
                portal_plot{i} = plot3(X,Y,Z,'g');
                set(portal_plot{i},'visible','on');
            endfor

            waitforbuttonpress;
        endif

    endwhile

    if hit,
        [v,lambda] = closest_pt(W);
        a = lambda * AW;
        b = lambda * BW;
    endif

    if DEBUG__,
        if hit,
            figure(1);
            hold on;
            plot3([a(1),b(1)],[a(2),b(2)],[a(3),b(3)],'g');
            plot3(a(1),a(2),a(3),'c*');
            plot3(b(1),b(2),b(3),'m*');

            figure(2);
            hold on;
            plot3(v(1),v(2),v(3),'b+','markersize',14);

            figure(3);
            A .-= v;
            plot_polyhedron(A,FA,'b');
            hold on;
            plot_polyhedron(B,FB,'r');
            axis equal;
        endif

        printf("number of iterations: %d\n\n",it);
    endif
end

function [wrong, n] = origin_vs_portal_candidate(w0,W)

    next = [2,3,1];
    wrong = 0;
    for i = 1:3,
        k = next(i);
        l = next(k);
        n = cross(W(i,:)-w0,W(k,:)-w0);
        s0 = sign(dot(n,-w0));
        swl = sign(dot(n,W(l,:)-w0));

        if s0 * swl < 0,    
            wrong = l;
            n = s0 * n;
            break;
        endif
    endfor

endfunction

% Choose the index which indicates the vertex in W that 
% should be replaced with w4
function ind = new_portal_vertex_index(w0,W,w4,O,check)
    next = [2,3,1];
    N = [
        cross(W(1,:) - w0, w4 - w0);
        cross(W(2,:) - w0, w4 - w0);
        cross(W(3,:) - w0, w4 - w0);
    ];

    S0 = [
        sign(dot(N(1,:), O - w4)),
        sign(dot(N(2,:), O - w4)),
        sign(dot(N(3,:), O - w4))
    ];

    DEBUG__ = false;

    plotter = {[],[]};
    for i = 1:3,
        k = next(i);
        l = next(k);
        
        swik = sign(dot(N(i,:),W(k,:)-w4));
        swki = sign(dot(N(k,:),W(i,:)-w4));

        if DEBUG__,
            plotter{1} = plot3(
                    [w0(1),W(i,1),w4(1),w0(1)],
                    [w0(2),W(i,2),w4(2),w0(2)],
                    [w0(3),W(i,3),w4(3),w0(3)],
                    'r--');
            plotter{2} = plot3(
                    [w0(1),W(k,1),w4(1),w0(1)],
                    [w0(2),W(k,2),w4(2),w0(2)],
                    [w0(3),W(k,3),w4(3),w0(3)],
                    'r--');
            set(plotter{1},'visible','on');
            set(plotter{2},'visible','on');
            waitforbuttonpress;
            set(plotter{1},'visible','off');
            set(plotter{2},'visible','off');
        endif

        if swik * S0(i) > 0 && swki * S0(k) > 0,
            % portal found
            if check,
                [wrong, n] = origin_vs_portal_candidate(w0,[W(i,:); W(k,:); w4]);
                if !wrong,
                    ind = l;
                    break;
                endif
            else
                ind = l;
                break;
            endif
        endif  
    endfor

endfunction

function [v,lambda] = closest_pt(W)

    A = [(W(2,:)-W(1,:))',(W(3,:)-W(1,:))'];
    ATA = A' * A;
    if det(ATA) < eps,
        v = [];
        lambda = [];
    endif

    x = -inv(ATA) * A' * W(1,:)';
    lambda = [1-x(1)-x(2),x(1),x(2)];
    v = lambda * W;

endfunction