#include <msp430.h> 
#include <stdlib.h>
#include <math.h>



#define BUFFER_SIZE 500
const int SPEED = 800;
const int SCALE = 60;
volatile unsigned int buffer_count = 0;
volatile unsigned char uart_buffer[BUFFER_SIZE];
volatile unsigned int buffer_head = 0;
volatile unsigned int buffer_tail = 0;
volatile _Bool ready = 1;
volatile int targetX = 0;
volatile int targetY = 0;
volatile int currentX = 0;
volatile int currentY = 0;
volatile double error = 0;
volatile char dominantAxis = 0;
volatile double slope = 0;
volatile int numStep = 0;
volatile char homing = 0;
volatile char xLim = 0;
volatile char yLim = 0;
volatile char dirX = 0;
volatile char dirY = 0;



void setup_UART(void);
void process_packet(void);
void add_to_buffer(unsigned char byte);
unsigned char get_from_buffer(void);
void send_UART_byte(unsigned char byte); // Function to send a single byte via UART
void setup_stepper_timer(void);
void homing_sequence(void);
void setup_encoder_pins(void);

/**
 *
 * main.c
 */
int main(void)
{
     WDTCTL = WDTPW | WDTHOLD;   // stop watchdog timer
    setup_UART();               // Setup UART
    setup_stepper_timer();
    setup_encoder_pins();

    P3DIR |= (BIT4 | BIT5 | BIT6 | BIT7);

    __bis_SR_register(GIE);     // Enable global interrupts

    // Continuously check for a new packet
    while(1){
        if (buffer_count < 60) {
            //send_UART_byte(0xFF);
        }
        if ((buffer_head - buffer_tail + BUFFER_SIZE) % BUFFER_SIZE >= 4)
        {
           if(ready) {
               send_UART_byte(0xFF);
               send_UART_byte(currentX/SCALE);
               send_UART_byte(currentY/SCALE);
               process_packet();  // Process if a full 4-byte packet is available
           }
        }
    }
}

void set_stepper_speed(unsigned int step_delay)
{
    TA1CCR0 = step_delay;                // Set Timer B delay
}

void setup_stepper_timer()
{
    P1DIR |= BIT6;
    TA1CCTL0 = CCIE;              // Enable Timer B interrupt
    TA1CTL = TBSSEL__ACLK | MC__UP | TBCLR | ID_2; // SMCLK, Up mode, clear timer
    set_stepper_speed(0);       // Set initial speed, e.g., 100 steps per second
}
#pragma vector = TIMER1_A0_VECTOR
__interrupt void Timer1_A0_ISR(void)
{
    P1OUT ^= BIT6;
    if(numStep > 0 && !homing){
        numStep--;
        switch (dominantAxis){
        case 'X':
            P3OUT |= BIT4; //P3.4 for X step
            currentX += (dirX)? 1:-1;
            error += slope;
            if(error >= 0.5){
                currentY += (dirY)? 1:-1;
                P3OUT |= BIT5; //P3.5 for Y step;
                error -= 1.0;
                P3OUT &= ~BIT5;
            }
            P3OUT &= ~BIT4;
            break;
        case 'Y':
            P3OUT |= BIT5; //P3.5 for Y step
            currentY += (dirY)? 1:-1;
            error += slope;
            if(error >= 0.5){
                currentX += (dirX)? 1:-1;
                P3OUT |= BIT4; //P3.4 for X step;
                error -= 1.0;
                P3OUT &= ~BIT4;
            }
            P3OUT &= ~BIT5;
            break;
        case 0:
            P3OUT |= (BIT4 | BIT5);
            currentX += (dirX)? 1:-1;
            currentY += (dirY)? 1:-1;
            P3OUT &= ~(BIT4 | BIT5);
            break;
        }
    }
    else if(homing) {return;}
    else ready = 1;


}

//UART FUNCTIONS

void process_packet(void)
{
    //buffer_count -= 6;
    ready = 0;
    volatile unsigned char start_byte, x, y, command;
    volatile unsigned int duty_cycle;
    volatile unsigned char dir, fX, fY, magOn, home;

    start_byte = get_from_buffer(); // Get start byte (255)
    if (start_byte != 255) return;  // Ignore if start byte isn't 255

    x = get_from_buffer();
    if (x == 255) return;
    y = get_from_buffer();
    if (y == 255) return;
    command = get_from_buffer();
    if (command == 255) return;

    //Command = 00ABCDEF = 0,0,home,MagOn,dxH,dxL,dyH,dyL
    if(command)
    {
        fY = command & BIT0; if(fY) y = 255;
        fX = command & BIT1; if(fX) x = 255;
        magOn = command & BIT4;
        home = command & BIT5;

    }

    if(magOn) P1OUT |= BIT0;
    else P1OUT &= ~BIT0;

    if(home)
    {
        homing_sequence();
        return;
    }
    else
    {

        targetX = x * SCALE;
        targetY = y * SCALE;

        //Implement Bresenham's line algorithm
        int dX = abs(targetX - currentX);
        int dY = abs(targetY - currentY);


        error = 0;

        double d = sqrt(pow(dX,2) + pow(dY,2));
        double t = d/SPEED;
        double interval = 0;
        if(dX > dY)
        {
            dominantAxis = 'X';
            slope = (double)dY/dX;
            interval = t/dX;
            numStep = dX;
        }
        else if(dX < dY)
        {
            dominantAxis = 'Y';
            slope = (double)dX/dY;
            interval = t/dY;
            numStep = dY;
        }
        else
        {
            dominantAxis = 0;
            slope = (double)dY/dX;
            interval = t/dX;
            numStep = dX;
        }

        int period = 750000 * interval;

        //Direction control
        if(currentX < targetX) {
            P3OUT &= ~BIT6;
            dirX = 1;
        }//
        else {
            P3OUT |= BIT6;
            dirX = 0;
        }//
        if(currentY < targetY) {
            P3OUT &= ~BIT7;
            dirY = 1;
        }//
        else {
            P3OUT |= BIT7;
            dirY = 0;
        }//

        set_stepper_speed(period);
    }


}

void homing_sequence(void){
    //Implement Homing sequence
        homing = 1;
        PJDIR |= BIT0;
        PJOUT |= BIT0;
        xLim = 0;
        yLim = 0;
        P3OUT |= BIT6;
        P3OUT |= BIT7;
        while(!xLim){
            P3OUT |= BIT4;
            _delay_cycles(24000);
            P3OUT &= ~BIT4;
            _delay_cycles(60000-24000); //0.01 second delay;
        }
        PJOUT &= ~BIT0;
        currentX = 0;
        while(!yLim){
            P3OUT |= BIT5;
            _delay_cycles(24000);
            P3OUT &= ~BIT5;
            _delay_cycles(60000-24000); //0.01 second delay;
        }
        PJOUT |= BIT0;
        currentY = 0;
        homing = 0;
        ready = 1;
        return;
}

void setup_encoder_pins(void)
{
    // Set P1.0 and P1.1 as input for the quadrature encoder
    P1DIR &= ~(BIT2 | BIT1);
    P1REN |= BIT2 | BIT1;        // Enable pull-up/pull-down resistors
    P1OUT |= BIT2 | BIT1;        // Set resistors as pull-up
    P1IES &= ~(BIT2 | BIT1);     // Trigger on rising edge initially
    P1IE |= BIT2 | BIT1;         // Enable interrupts on P1.0 and P1.1
}

// Port 1 interrupt service routine for encoder
#if defined(__TI_COMPILER_VERSION__) || defined(__IAR_SYSTEMS_ICC__)
#pragma vector=PORT1_VECTOR
__interrupt void Port_1(void)
#elif defined(__GNUC__)
void __attribute__ ((interrupt(PORT1_VECTOR))) Port_1 (void)
#else
#error Compiler not supported!
#endif
{

    PJDIR |= BIT1 | BIT2;

    if(P1IFG & BIT1) xLim = 1;
    else if(P1IFG & BIT2) yLim = 1;
    P1IFG &= ~(BIT2 | BIT1);  // Clear interrupt flags for P1.0 and P1.1
}

void setup_UART(void)
{
    // Set DCO to 24 MHz
    CSCTL0_H = 0xA5;
    CSCTL1 |= DCORSEL + DCOFSEL0 + DCOFSEL1; // Set max. DCO setting
    CSCTL2 = SELA_3 + SELS_3 + SELM_3; //
    CSCTL3 = DIVA_3 + DIVS_0 + DIVM_0; //
    CSCTL0_H = 0;                 // Lock CS registers



    P2SEL1 |= BIT0 + BIT1;
    P2SEL0 &= ~(BIT0 + BIT1);


    UCA0CTLW0 |= UCSWRST;    // Put UART in reset
    UCA0CTLW0 |= UCSSEL__SMCLK; // SMCLK as clock source

    UCA0MCTLW |= 0x0000;
    UCA0BR0 = 0xc4; // 9600 baud
    UCA0BR1 = 0x09;

    UCA0CTLW0 &= ~UCSWRST;   // Release UART from reset
    UCA0IE |= UCRXIE;        // Enable UART receive interrupt

}

// UART ISR
// UART interrupt service routine
#if defined(__TI_COMPILER_VERSION__) || defined(__IAR_SYSTEMS_ICC__)
#pragma vector=USCI_A0_VECTOR
__interrupt void USCI_A0_ISR(void)
#elif defined(__GNUC__)
void __attribute__ ((interrupt(USCI_A0_VECTOR))) USCI_A0_ISR (void)
#else
#error Compiler not supported!
#endif
{
    unsigned char received_byte = UCA0RXBUF;  // Read received byte
    add_to_buffer(received_byte);             // Add to circular buffer
}

// Function to send a single byte over UART
void send_UART_byte(unsigned char c)
{
    while (!(UCA0IFG & UCTXIFG));    // Wait until the transmit buffer is empty
    UCA0TXBUF = c;                   // Transmit the byte
}

void add_to_buffer(unsigned char byte)
{
    buffer_count++;
    uart_buffer[buffer_head] = byte;
    buffer_head = (buffer_head + 1) % BUFFER_SIZE;
}

unsigned char get_from_buffer(void)
{
    unsigned char byte = uart_buffer[buffer_tail];
    buffer_tail = (buffer_tail + 1) % BUFFER_SIZE;
    return byte;
}

