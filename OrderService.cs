using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Web.Infrastructure
{
    using Models;
    using System.Data.SqlClient;

    public class OrderService
    {
        public List<Order> GetOrdersForCompany(int CompanyId)
        {
            var OrderList = new List<Order>();
            var OrderProductList = new List<OrderProduct>();
            var database = new Database();
            List<SqlParameter> paralist = new List<SqlParameter>();
            SqlParameter pCompid = new SqlParameter("@companyid", CompanyId);
            paralist.Add(pCompid);

            try
            {
                // Get the orders
                var qGetOrders = "SELECT c.name, o.description, o.order_id FROM company c INNER JOIN [order] o on c.company_id=o.company_id where c.company_id=@companyid";

                var reader1 = database.ExecuteReader(qGetOrders, paralist);
                while (reader1.Read())
                {
                    var record1 = (IDataRecord)reader1;
                    OrderList.Add(new Order()
                    {
                        CompanyName = record1.GetString(0),
                        Description = record1.GetString(1),
                        OrderId = record1.GetInt32(2),
                        OrderProducts = new List<OrderProduct>()
                    });
                }
                reader1.Close();
                database.CloseConnection();

                //Get the order products
                var sql2 = "select op.price,o.order_id,  op.product_id, op.quantity, p.name, p.price from company c join [order] o on c.company_id = o.company_id join orderproduct op on o.order_id = op.order_id join product p on op.product_id = p.product_id where o.company_id = @companyid order by o.order_id";

                var reader2 = database.ExecuteReader(sql2, paralist);
                while (reader2.Read())
                {
                    var record2 = (IDataRecord)reader2;
                    OrderProductList.Add(new OrderProduct()
                    {
                        OrderId = record2.GetInt32(1),
                        ProductId = record2.GetInt32(2),
                        Price = record2.GetDecimal(0),
                        Quantity = record2.GetInt32(3),
                        Product = new Product()
                        {
                            Name = record2.GetString(4),
                            Price = record2.GetDecimal(5)
                        }
                    });
                }

                reader2.Close();
                database.CloseConnection();

                foreach (var order in OrderList)
                {
                    var getproductdetails = OrderProductList.Where(id => id.OrderId.Equals(order.OrderId));
                    if (getproductdetails != null)
                    {
                        foreach (var op in getproductdetails)
                        {
                            order.OrderProducts.Add(new OrderProduct() { OrderId = op.OrderId, Price = op.Price, Product = op.Product, Quantity = op.Quantity, ProductId = op.ProductId });
                            order.OrderTotal += op.Price * op.Quantity;
                        }
                    }
                }
                return OrderList;
            }
            catch (Exception ex)
            {
                var message = ex.Message != null ? ex.Message.ToString() : ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString();
                // Log into ServiceLogger table
            }
            return null;
        }
    }
}