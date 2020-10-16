% Expanding-polytope algorithm in 3D
function [v,a,b] = epa3d(A,B,FA,FB,AdjA,AdjB,W,AW,BW)

    DEBUG__ = true;

    if DEBUG__,
        figure(1);
        axis equal;
        plot_polyhedron(A,FA,'b');
        hold on;
        plot_polyhedron(B,FB,'r');

        figure(2);
        axis equal;
        [CSO,FCSO] = cso3d(A,B);
        % plot_polyhedron(CSO,FCSO,[0.5,0.5,0.5]);
        hold on;
        waitforbuttonpress;
    endif

    pq = pqueue(@(e1,e2) e1.d2 > e2.d2);
    [W,FW,AW,BW,AdjFW,IndFW] = construct_init_tetrahedron(W,A,FA,AW,B,FB,BW);

    if DEBUG__,
        plot_polyhedron(W,FW,'g');
        hold on;
        waitforbuttonpress;
        % plot_polyhedron_normals(W,FW,'r');
    endif

    init_entries = {[],[],[],[]};
    for i = 1:4,
        init_entries{i} = construct_entry(
            W(FW(i,1),:),  W(FW(i,2),:),  W(FW(i,3),:),
            AW(FW(i,1),:), AW(FW(i,2),:), AW(FW(i,3),:),
            BW(FW(i,1),:), BW(FW(i,2),:), BW(FW(i,3),:)
        );
    endfor

    for i = 1:4,
        init_entries{i}.adj{1} = init_entries{AdjFW(i,1)};
        init_entries{i}.adj{2} = init_entries{AdjFW(i,2)};
        init_entries{i}.adj{3} = init_entries{AdjFW(i,3)};
        init_entries{i}.ind = IndFW(i,:);
        
        % init_entries{i}

        if closest_is_internal(init_entries{i})
            pq.push(init_entries{i});
        endif
    endfor

    max_it = 20;
    it = 0;
    mu = Inf;
    while max_it,
        entry = pq.pop();

        if !entry.obs,
            v = entry.v;
            d2 = entry.d2;
            aw = support_mapping3d(A,AdjA,v);
            bw = support_mapping3d(B,AdjB,-v);
            w = aw - bw;
            mu = min(mu,dot(w,v)^2 / d2);

            if DEBUG__,
                figure(2);
                hold on;
                plot3(v(1),v(2),v(3),'ko');
                plot3(w(1),w(2),w(3),'ro');
                waitforbuttonpress;
            endif

            if mu <= (1 + eps)^2 * d2
                printf("v is close enough\n");
                printf("number of iterations: %d\n",it)
                break;
            endif

            entry.obs = true;
            E = {}; % empty set of entries
            for i = 1:3,
                E = silhouette(entry.adj{i},entry.ind(i),w,E);
            endfor

            % construct the new entries from the silhouette
            m = length(E);
            new_entries = cell(m,1);
            entry_fail = false;
            for i = 1:m,
                j = E{i}{2};
                next = mod(j,3) + 1;

                new_entry = construct_entry(               
                    E{i}{1}.y(j,:),w, E{i}{1}.y(next,:),
                    E{i}{1}.p(j,:),aw,E{i}{1}.p(next,:),
                    E{i}{1}.q(j,:),bw,E{i}{1}.q(next,:)
                );

                if isempty(new_entry),
                    entry_fail = true;
                    break;
                endif

                new_entries{i} = new_entry;
            endfor

            if entry_fail,
                printf("calculating new entry failed\n");
                printf("number of iterations: %d\n\n",it);
                break;
            endif
            
            % adjust adjacency information
            for i = 1:m,
                new_entries{i}.adj{1} = new_entries{mod(i,m) + 1};
                new_entries{i}.adj{2} = new_entries{mod(i-2,m) + 1};
                new_entries{i}.adj{3} = E{i}{1};
                new_entries{i}.ind = [2,1,E{i}{2}];

                E{i}{1}.adj{E{i}{2}} = new_entries{i};
                E{i}{1}.ind(E{i}{2}) = 3;

                if closest_is_internal(new_entries{i}) && d2 <= new_entries{i}.d2 && new_entries{i}.d2 <= mu,
                    pq.push(new_entries{i});
                endif

                if DEBUG__
                    col = rand(3,1);
                    figure(2);
                    new_entries{i}.plot_entry(col);
                    waitforbuttonpress;
                endif
            endfor

            if DEBUG__,
                for i = 1:m, new_entries{i}.plot_entry('g');
                endfor
                
                it++;
            endif
               
        endif
    endwhile

    v = entry.v;
    a = entry.lambda * entry.p;
    b = entry.lambda * entry.q;

    if DEBUG__,
        printf("a - b = \n\n%s\n",disp(a-b));

        figure(1);
        hold on;
        plot3([a(1),b(1)],[a(2),b(2)],[a(3),b(3)],'g');

        figure(2);
        hold on;
        plot3(v(1),v(2),v(3),'g*');
        [CSO,FCSO] = cso3d(A,B);
        plot_polyhedron(CSO,FCSO,'k--');

        figure(3);
        A .-= v;
        plot_polyhedron(A,FA,'b');
        hold on;
        plot_polyhedron(B,FB,'r');
    endif

endfunction

function entry = construct_entry(y1,y2,y3,p1,p2,p3,q1,q2,q3)

    A = [(y2 - y1)', (y3 - y1)'];
    ATA = A' * A;

    if det(ATA) < eps,
        entry = [];
        return;
    endif

    x = -inv(ATA) * A' * y1';

    lambda = [1-x(1)-x(2),x(1),x(2)];
    v = lambda(1) * y1 + lambda(2) * y2 + lambda(3) * y3;
    d2 = dot(v,v);

    entry = entry3d(
        [y1;y2;y3],
        [p1;p2;p3],
        [q1;q2;q3],
        v,
        d2,
        lambda
    );

endfunction

function [W,FW,AW,BW,AdjFW,IndFW] = construct_init_tetrahedron(W,A,FA,AW,B,FB,BW)

    switch length(W)
        
        case 1,
            printf("\n touching contacts \n\n");
        case 2,
            printf("\n should have implemented this one\n\n")
            % TODO: construct a hexahedron from the line segment,
            % then choose the tetrahedron which contains the origin
        case 3,
            n = cross(W(2,:) - W(1,:), W(3,:) - W(1,:));
            % TODO: construct a hexahedron from the triangle,
            % then choose the tetrahedron which contains the origin
        case 4,
            % construct a tetrahedron with counterclockwise oriented faces
            n = cross(W(2,:) - W(1,:), W(3,:) - W(1,:));
            if dot(W(4,:) - W(1,:),n) > 0,
                [W(3,:),W(2,:)] = deal(W(2,:),W(3,:));
                [AW(3,:),AW(2,:)] = deal(AW(2,:),AW(3,:));
                [BW(3,:),BW(2,:)] = deal(BW(2,:),BW(3,:));
            endif

            % magic numbers
            FW = [
                1, 2, 3;
                1, 3, 4;
                3, 2, 4;
                1, 4, 2;
            ];
            
            AdjFW = [
                4, 3, 2;
                1, 3, 4;
                1, 4, 2;
                2, 3, 1
            ];

            IndFW = [
                3, 1, 1;
                3, 3, 1;
                2, 2, 2;
                3, 2, 1
            ];
        
    end %switch

endfunction

function b = closest_is_internal(entry)

    b = (-eps <= entry.lambda(1) && 
        -eps <= entry.lambda(2) &&
        -eps <= entry.lambda(3));

endfunction

% An "ordered" DFS algorithm for finding the triangles on the polytope
% which are not visible from w
function E = silhouette(entry,i,w,E)

    % check if we saw this entry before
    if entry.obs, return
    endif

    if dot(entry.v,w) < entry.d2,
        % entry is not visible from w
        E{end+1} = {entry,i};
    else
        entry.obs = true;
        E = silhouette(entry.adj{mod(i,3) + 1},entry.ind(mod(i,3) + 1),w,E);
        E = silhouette(entry.adj{mod(i+1,3) + 1},entry.ind(mod(i+1,3)+1),w,E);
    endif


endfunction