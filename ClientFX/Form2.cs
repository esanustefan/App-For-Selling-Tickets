using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Services;
using ConcursBaschet.domain;
using ConsoleApplication1;
using Match = ConcursBaschet.domain.Match;

namespace Baschet_Server_Client3
{
    public partial class Form2 : Form, ObserverInterface
    {
        private ServiceInterface service;

        public Form2(ServiceInterface serviceApp) 
        {
            InitializeComponent();
            this.service = serviceApp;
        }
        
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //get the selected match from the datagridview
            // int matchId = (int)dataGridView1.Rows[e.RowIndex].Cells[0].Value;
            // Console.WriteLine("Aceste este meciul : ____________________ " + matchId);
            // Match match = service.findOneMatch(matchId);
            
            //open form3
            // Form3 form3 = new Form3(service, match);
            // // form3.Show();
            // this.Hide();
        }
        
        private void only_availableBtn_Click(object sender, EventArgs e)
        {
            emptyTable();
            foreach (Match match in service.getAllMatches(this))
            {
                // call myService.CheckAvailableSeats() to check if seats are available
                MatchNoTicketsDTO matchNoTicketsDto = new MatchNoTicketsDTO(match, 0);
                int seatsAvailable = service.CheckAvailableSeats(matchNoTicketsDto);
                if (seatsAvailable > 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridView1);
                    row.Cells[0].Value = match.GetId();
                    row.Cells[1].Value = match.TeamA;
                    row.Cells[2].Value = match.TeamB;
                    row.Cells[3].Value = match.TicketPrice;
                    row.Cells[4].Value = match.SoldSeats;
                    row.Cells[5].Value = "Available";
                    row.Cells[5].Style.ForeColor = Color.Green;
                    dataGridView1.Rows.Add(row);
                }
            }
        }

        private void refreshBtnClick(object sender, EventArgs e)
        {
            emptyTable();
            initializeDataGridView();
        }

        private void Inapoi_Click(object sender, EventArgs e)
        {
            emptyTable();
            initializeDataGridView();
        }
        
        //method to initialize the datagridview with the matches
        public void initializeDataGridView()
        {
            foreach (Match match in service.getAllMatches(this))
            {
                // call myService.CheckAvailableSeats() to check if seats are available
                MatchNoTicketsDTO matchNoTicketsDto = new MatchNoTicketsDTO(match, 0);
                int seatsAvailable = service.CheckAvailableSeats(matchNoTicketsDto);
                Console.WriteLine("Seats available for match " + match.GetId() + match.TeamA + ": " + seatsAvailable);
                // add a row to the DataGridView
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = match.GetId();
                row.Cells[1].Value = match.TeamA;
                row.Cells[2].Value = match.TeamB;
                row.Cells[3].Value = match.TicketPrice;
                row.Cells[4].Value = match.SoldSeats;

                // add "Available" or "Sold out" column based on seats availability
                if (seatsAvailable > 0)
                {
                    row.Cells[5].Value = "Available";
                    row.Cells[5].Style.ForeColor = Color.Green;
                }
                else
                {
                    row.Cells[5].Value = "Sold out";
                    row.Cells[5].Style.ForeColor = Color.Red;
                }

                // add the row to the DataGridView
                dataGridView1.Rows.Add(row);


            }
        }
        
        private void emptyTable()
        {
            dataGridView1.Rows.Clear();
        }
        

        public void updateTickets()
        {
            dataGridView1.BeginInvoke(new Action((() =>
            {
                emptyTable();
                initializeDataGridView();
            })));
        }

        public void updateMatches()
        {
            dataGridView1.BeginInvoke(new Action((() =>
            {
                emptyTable();
                initializeDataGridView();
            })));
        }

        private void BuyTicket_Click(object sender, EventArgs e)
        {
            
            //get the selected match from the datagridview
            int matchId = (int)dataGridView1.SelectedRows[0].Cells[0].Value;
            Console.WriteLine("Aceste este meciul : ____________________ " + matchId);
            Match match = service.findOneMatch(matchId);
            
            
            //get the number of tickets from the textbox
            
            int numberOfTickets;
            
            try
            {
                numberOfTickets = Int32.Parse(NumberOfTicketsTF.Text);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Please enter a valid number of tickets!");
                return;
            }
            
            //check if the number of tickets is valid
            if (numberOfTickets <= 0)
            {
                MessageBox.Show("Please enter a valid number of tickets!");
                return;
            }
            
            //get the name of the buyer from the textbox
            string buyerName = NameOfClientTF.Text;
            
            //check if the name of the buyer is valid
            if (buyerName.Length < 3)
            {
                MessageBox.Show("Please enter a valid name!");
                return;
            }
            
            //buy the tickets
            MatchNoTicketsDTO matchNoTicketsDto = new MatchNoTicketsDTO(match, numberOfTickets);
            TicketDTO ticketDto = new TicketDTO(matchNoTicketsDto, buyerName, numberOfTickets);
            try
            {
                service.buyTickets(ticketDto, this);
                MessageBox.Show("Tickets bought successfully!");
                // updateTickets();
                // updateMatches();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}