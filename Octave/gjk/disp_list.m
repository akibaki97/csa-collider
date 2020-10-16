function disp_list(L)

    elem = L.next;
    while !isempty(elem)
        disp(elem.data);
        elem = elem.next;
    endwhile

endfunction