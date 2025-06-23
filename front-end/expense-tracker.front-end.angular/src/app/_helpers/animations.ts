import { trigger, style, transition, animate } from '@angular/animations';

export const fadeDropdown = trigger('fadeDropdown', [
    transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.9) translateY(-10px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'scale(1) translateY(0)' }))
    ]),
    transition(':leave', [
        animate('200ms ease-in', style({ opacity: 0, transform: 'scale(0.9) translateY(-10px)' }))
    ])
]);