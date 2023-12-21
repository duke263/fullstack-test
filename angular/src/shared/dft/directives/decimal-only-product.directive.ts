/* eslint-disable */
import { Directive, ElementRef, AfterViewInit, Input, EventEmitter, Output } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
    selector: '[decimalProduct]'
})
export class DecimalProductDirective implements AfterViewInit {

    // tslint:disable-next-line:no-input-rename
    @Input('decimalProduct') decimal: any;
    @Input() max: number;
    @Output() onValueChange: EventEmitter<string> = new EventEmitter();
    private _isInitialized = false;

    // tslint:disable-next-line:naming-convention
    constructor(private _element: ElementRef, private control: NgControl) {

    }

    // tslint:disable-next-line:cognitive-complexity
    ngAfterViewInit() {
        if (this._isInitialized) {
            return;
        }

        $(this._element.nativeElement).inputmask('numeric', {
            digits: this.decimal !== '' && this.decimal['digits'] ? this.decimal['digits'] : 0,
            integerDigits: this.decimal !== '' && this.decimal['integerDigits'] ? this.decimal['integerDigits'] : 16,
            groupSeparator: this.decimal !== '' && this.decimal['groupSeparator'] ? this.decimal['groupSeparator'] : '.',
            radixPoint: this.decimal !== '' && this.decimal['radixPoint'] ? this.decimal['radixPoint'] : '.',
            autoGroup: this.decimal !== '' && this.decimal['autoGroup'] ? this.decimal['autoGroup'] : true,
            rightAlign: this.decimal !== '' && this.decimal['rightAlign'] ? this.decimal['rightAlign'] : false,
            autoUnmask: this.decimal !== '' && this.decimal['autoUnmask'] ? this.decimal['autoUnmask'] : true,
            prefix: this.decimal !== '' && this.decimal['prefix'] ? this.decimal['prefix'] : '',
            allowPlus: this.decimal !== '' && this.decimal['allowPlus'] !== undefined ? this.decimal['allowPlus'] : true,
            allowMinus: this.decimal !== '' && this.decimal['allowMinus'] !== undefined ? this.decimal['allowMinus'] : false,
            oncomplete: (value) => {
                const start = value.target.selectionStart;
                const end = value.target.selectionEnd;
                const dot = this.decimal !== '' && this.decimal['digits'] && this.decimal['radixPoint'] !== undefined ? this.decimal['radixPoint'] : '.';
                if (this._element.nativeElement.value[this._element.nativeElement.value.length - 1] !== dot) {
                    this.control.control.setValue(Number(this._element.nativeElement.value));
                    value.target.selectionStart = start;
                    value.target.selectionEnd = end;
                }
            }, oncleared: () => {
                this.control.control.setValue(null);
            }
        });

        this._isInitialized = true;
    }
}
