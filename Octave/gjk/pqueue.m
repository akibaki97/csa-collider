## -*- texinfo -*-
## @deftp {Class} pqueue
## A totally unefficient implementation of the priority queue containeer.
## Also user-unfriendly af.
## @end deftp

classdef pqueue

  properties
    head;
    less;
  endproperties

  methods

    function this = pqueue(less)
        this.head = lnode();
        this.less = less;
    endfunction

    function this = push(this,data)
        v = this.head.next;
        this.head.next = lnode(data);
        this.head.next.next = v;
    endfunction

    function data = pop(this)
        if isempty(this.head.next),
            error("empty queue");
        endif

        v = this.head;
        max_val = v.next.data;
        max_ref = v;
        v = v.next;
        while !isempty(v.next),
            if this.less(max_val,v.next.data),
                max_val = v.next.data;
                max_ref = v;
            endif

            v = v.next;
        endwhile

        max_ref.next = max_ref.next.next;
        data = max_val;
    endfunction

    function b = is_empty(this)
        b = isempty(this.head.next);
    endfunction

    function disp(this)
        v = this.head.next;

        while !isempty(v),
            disp(v.data);
            v = v.next;
        endwhile
    endfunction

  endmethods

endclassdef