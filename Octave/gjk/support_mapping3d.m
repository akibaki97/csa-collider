function [w,i] = support_mapping3d(V,A,v)

    % init state
    n = rows(V);
    marked = zeros(n);
    marked(1) = true;
    max_proj = dot(V(1,:),v);
    support_found = false;
    curr = 1;

    % hill climbing
    while !support_found,
        max_adj = 0;
        support_found = true;
        adj = A(curr).next;
        
        while !isempty(adj),
            if !marked(adj.data),
                proj = dot(V(adj.data,:),v);
                if proj >= max_proj,
                    max_proj = proj;
                    max_adj = adj.data;
                    support_found = false;
                endif
                
                marked(adj.data) = true;
            endif

            adj = adj.next;
        endwhile

        if !support_found, curr = max_adj;
        endif
    endwhile

    i = curr;
    w = V(curr,:);

endfunction